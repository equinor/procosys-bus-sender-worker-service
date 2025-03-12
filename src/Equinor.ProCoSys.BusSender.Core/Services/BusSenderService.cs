using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.BusSenderWorker.Core.Extensions;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Mappers;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class BusSenderService : IBusSenderService
{
    private readonly IBusEventRepository _busEventRepository;
    private readonly ILogger<BusSenderService> _logger;
    private readonly IPcsBusSender _pcsBusSender;
    private readonly IBusEventService _service;
    private readonly IQueueMonitorService _queueMonitor;
    private readonly Stopwatch _sw;
    private readonly ITelemetryClient _telemetryClient;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlobLeaseService _blobLeaseService;
    private readonly IPlantService _plantService;
    private bool _hasPendingEventsForCurrentPlant = false;

    public BusSenderService(IPcsBusSender pcsBusSender,
        IBusEventRepository busEventRepository,
        IUnitOfWork unitOfWork,
        ILogger<BusSenderService> logger,
        ITelemetryClient telemetryClient,
        IBusEventService service,
        IQueueMonitorService queueMonitor,
        IBlobLeaseService blobLeaseService,
        IPlantService plantService)
    {
        _pcsBusSender = pcsBusSender;
        _busEventRepository = busEventRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _telemetryClient = telemetryClient;
        _service = service;
        _queueMonitor = queueMonitor;
        _sw = new Stopwatch();
        _blobLeaseService = blobLeaseService;
        _plantService = plantService;
    }

    public async Task CloseConnections()
    {
        _logger.LogInformation("BusSenderService stop reader and close all connections");
        await _pcsBusSender.CloseAllAsync();
    }

    public bool HasPendingEventsForCurrentPlant() => _hasPendingEventsForCurrentPlant;

    public async Task HandleBusEvents()
    {
        PlantLease? plantLease = null;
        try
        {
            _sw.Start();
            _hasPendingEventsForCurrentPlant = false;
            var plantLeases = await _blobLeaseService.ClaimPlantLease();
            if (plantLeases == null)
            {
                _logger.LogDebug("No plant lease available exiting.");
                return;
            }

            plantLease = plantLeases?.FirstOrDefault(x => x.IsCurrent);
  
            if (plantLeases == null || plantLease?.Plant == null)
            {
                _logger.LogDebug("No plant leases available exiting.");
                return;
            }

            if (ReleasePlantLeaseIfExpired(plantLease))
            {
                return;
            }

            var plants = _plantService.GetPlantsForCurrent(plantLeases);

            await _queueMonitor.WriteQueueMetrics();

            _busEventRepository.SetPlants(plants);
            var events = await _busEventRepository.GetEarliestUnProcessedEventChunk();
            if (events.Any())
            {
                var plant = plantLease.Plant;
                _logger.LogInformation(
                    "[{Plant}] BusSenderService found {Count} messages to process after {Sw} ms", plant,
                    events.Count,
                    _sw.ElapsedMilliseconds);
                _telemetryClient.TrackMetric("BusSender Chunk", events.Count);
                await ProcessBusEvents(events, plant);
                _logger.LogInformation("[{Plant}] BusSenderService ProcessBusEvents used {Sw} ms", plant,
                    _sw.ElapsedMilliseconds);
            }

            // Release plant lease if it has expired.
            if (!ReleasePlantLeaseIfExpired(plantLease))
            {
                // ... or if there are no more unprocessed events for this plant.
                await ReleasePlantLeaseIfProcessingCompleted(plantLease);
            }

            _logger.LogInformation($"BusSenderService ProcessBusEvents used {_sw.ElapsedMilliseconds} ms"); 
            _sw.Reset();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "BusSenderService execute send failed");
            _blobLeaseService.ReleasePlantLease(plantLease);
            throw;
        }

        _logger.LogDebug("BusSenderService DoWorkerJob finished");
        _telemetryClient.Flush();
    }

    public async Task HandleBusEventsSingleInstance()
    {
        try
        {
            await _queueMonitor.WriteQueueMetrics();

            _sw.Start();
            var events = await _busEventRepository.GetEarliestUnProcessedEventChunk();
            if (events.Any())
            {
                _logger.LogInformation("BusSenderService found {Count} messages to process after {Sw} ms", events.Count,
                    _sw.ElapsedMilliseconds);
                _telemetryClient.TrackMetric("BusSender Chunk", events.Count);
                await ProcessBusEvents(events);
                _logger.LogInformation("BusSenderService ProcessBusEvents used {Sw} ms", _sw.ElapsedMilliseconds);
            }

            _sw.Reset();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "BusSenderService execute send failed");
            throw;
        }

        _logger.LogDebug("BusSenderService DoWorkerJob finished");
        _telemetryClient.Flush();
    }


    private bool ReleasePlantLeaseIfExpired(PlantLease? plantLease)
    {
        if (plantLease != null && plantLease.IsExpired())
        {
            _logger.LogDebug("Lease has expired for plant: {Plant}. Releasing it.", plantLease.Plant);
            _blobLeaseService.ReleasePlantLease(plantLease);
            return true;
        }

        return false;
    }

    private async Task ReleasePlantLeaseIfProcessingCompleted(PlantLease plantLease)
    {
        var remainingEvents = await _busEventRepository.GetEarliestUnProcessedEventChunk();
        if (remainingEvents.Any())
        {
            _logger.LogDebug("[{Plant}] More unprocessed events are handled in the next loop by this instance. Keeping blob lease for this plant.", plantLease.Plant);
            _hasPendingEventsForCurrentPlant = true;
        }
        else
        {
            _logger.LogDebug("[{Plant}] No more unprocessed events for this plant. Releasing blob lease.", plantLease.Plant);
            _blobLeaseService.ReleasePlantLease(plantLease);
            _hasPendingEventsForCurrentPlant = false;
        }
    }

    private async Task BatchAndSendPerTopic(List<(string Key, Queue<BusEvent> messages)> eventGroups)
    {
        foreach (var (topic, messages) in eventGroups)
        {
            /***
            * group messages into batches, and fail before sending if message exceeds size limit
            * Pattern taken from:
            * https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/MigrationGuide.md
            */
            var messageCount = messages.Count;
            while (messages.Count > 0)
            {
                using var messageBatch = await _pcsBusSender.CreateMessageBatchAsync(topic);
                // add first unsent message to batch
                
                if (TryAddMessage(messageBatch, messages, out var msgId, out var msgBody))
                {
                    var m = messages.Dequeue();
                    m.Status = Status.Sent;
                    TrackMessage(m, msgId, msgBody);
                }
                else
                {
                    // if the first message can't fit, then it is too large for the batch
                    throw new Exception($"Message {messageCount - messages.Count} is too large and cannot be sent.");
                }
                
                while (messages.Count > 0 && TryAddMessage(messageBatch, messages, out var messageId, out var messageBody))
                {
                    var m = messages.Dequeue();
                    m.Status = Status.Sent;
                    TrackMessage(m,messageId, messageBody);
                }

                _logger.LogDebug("Sending amount: {Count} after {Ms} ms", messageBatch.Count, _sw.ElapsedMilliseconds);
                await _pcsBusSender.SendMessagesAsync(messageBatch, topic);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogDebug("done sending and save after {Ms} ms", _sw.ElapsedMilliseconds);
            }
        }
    }

    private bool TryAddMessage(ServiceBusMessageBatch messageBatch, Queue<BusEvent> messages, out string messageId, out string body)
    {
        var serviceBusMessage = new ServiceBusMessage(_service.WashString(messages.Peek().MessageToSend ?? messages.Peek().Message));
        serviceBusMessage.MessageId = Guid.NewGuid().ToString();
        if (serviceBusMessage.Body == null || string.IsNullOrEmpty(serviceBusMessage.Body.ToString()))
        {
            body = "Empty";
            _logger.LogError("MessageBody is null for {MessageId}", serviceBusMessage.MessageId);
        }
        else
        {
            body = serviceBusMessage.Body.ToString();
        }
        messageId = serviceBusMessage.MessageId;
        return messageBatch.TryAddMessage(
            serviceBusMessage);
    }

    /***
     * Takes a function to create a message, caller needs to make sure that event has the correct topic for the function.
     */
    private static async Task CreateAndSetMessage(BusEvent busEvent, Func<string, Task<string?>> createMessageFunction)
    {
        var message = await createMessageFunction(busEvent.Message);
        if (string.IsNullOrEmpty(message) || message == "null")
        {
            busEvent.Status = Status.NotFound;
        }
        else
        {
            busEvent.MessageToSend = message;
        }
    }

    /// <summary>
    ///     Removes all events that have Message = more than just a simple Id that can be converted to a long.
    ///     Then groups the events into bulks based on Id and Event type.
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    private static IEnumerable<IGrouping<(string Event, string Message), BusEvent>>
        FilterOnSimpleMessagesAndGroupDuplicates(
            IEnumerable<BusEvent> events)
        => events.Where(IsSimpleMessage)
            .GroupBy(e => (e.Event, e.Message));

    private static bool IsSimpleMessage(BusEvent busEvent)
        => long.TryParse(busEvent.Message, out _)
           || Guid.TryParse(busEvent.Message, out _)
           || BusEventService.CanGetTwoIdsFromMessage(busEvent.Message.Split(","), out _, out _);

    private async Task ProcessBusEvents(List<BusEvent> events, string plant = "ALL")
    {
        events = SetDuplicatesToSkipped(events);
        var dsw = Stopwatch.StartNew();

        var unProcessedEvents = events.Where(busEvent => busEvent.Status == Status.UnProcessed).ToList();
        _logger.LogInformation("[{Plant}] Amount of messages to process: {Count} ", plant, unProcessedEvents.Count);

        foreach (var simpleUnprocessedBusEvent in unProcessedEvents.Where(e =>
                     IsSimpleMessage(e) || e.Event == TagTopic.TopicName))
        {
                await UpdateEventBasedOnTopic(simpleUnprocessedBusEvent);
        }

        _logger.LogInformation("[{Plant}] Update loop finished at {Sw} ms", plant, dsw.ElapsedMilliseconds);
        if (!_blobLeaseService.CancellationToken.IsCancellationRequested)
        {
            await _unitOfWork.SaveChangesAsync();
        }
        else
        {
            _logger.LogWarning("[{Plant}] SaveChangesAsync skipped due to BlobLeaseExpiryTime.", plant);
            return;
        }
        /***
         * Group by topic and then create a queue of messages per topic
         */
        var topicQueues = events.Where(busEvent => busEvent.Status == Status.UnProcessed)
            .GroupBy(e => BusEventToTopicMapper.Map(e.Event)).Select(group =>
            {
                Queue<BusEvent> messages = new();
                group.ToList().ForEach(be => messages.Enqueue(be));
                return (group.Key, messages);
            }).ToList();

        await BatchAndSendPerTopic(topicQueues);
    }

    private static void SetAllButOneEventToSkipped(IEnumerable<BusEvent> group)
    {
        foreach (var busEvent in group.SkipLast(1))
        {
            busEvent.Status = Status.Skipped;
        }
    }

    private static List<BusEvent> SetDuplicatesToSkipped(List<BusEvent> events)
    {
        var duplicateGroupings = FilterOnSimpleMessagesAndGroupDuplicates(events);
        foreach (var group in duplicateGroupings)
        {
            SetAllButOneEventToSkipped(group);
        }

        return events;
    }

    private void TrackMessage(BusEvent busEvent, string busMessageMessageId, string busMessageBody)
    {
        var busEventMessageToSend = busEvent.MessageToSend ?? busEvent.Message;

        var message = JsonSerializer.Deserialize<BusEventMessage>(_service.WashString(busEventMessageToSend)!,
            DefaultSerializerHelper.SerializerOptions);
        
        if (message != null && string.IsNullOrEmpty(message.ProjectName))
        {
            message.ProjectName = "_";
        }
        _telemetryClient.TrackEvent("BusSender Message", new Dictionary<string, string>
        {
            {"Event", busEvent.Event},
            {"ProCoSysGuid", message?.ProCoSysGuid ?? "NoGuid!!???"},
            {"Created", busEvent.Created.ToString(CultureInfo.InvariantCulture)},
            {"ProjectName", message?.ProjectName ?? "NoProject"},
            {"Plant", message?.Plant ?? "NoPlant"},
            {"MessageId", busMessageMessageId ?? "NoID" }
        });
    }
    
    private async Task UpdateEventBasedOnTopic(BusEvent busEvent)
    {
        _logger.LogDebug("Started update for {Event}", busEvent.Event);
        var sw = Stopwatch.StartNew();
        switch (busEvent.Event)
        {
            case PersonTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreatePersonMessage);
                    break;
                }
            case HeatTraceTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateHeatTraceMessage);
                    break;
                }
            case TagTopic.TopicName:
                {
                    busEvent.Message = await _service.AttachTagDetails(busEvent.Message);
                    break;
                }
            case "checklist":
                {
                    await CreateAndSetMessage(busEvent, _service.CreateChecklistMessage);
                    break;
                }
            case CommPkgQueryTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateCommPkgQueryMessage);
                    break;
                }
            case CommPkgOperationTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateCommPkgOperationMessage);
                    break;
                }
            case QueryTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateQueryMessage);
                    break;
                }
            case QuerySignatureTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateQuerySignatureMessage);
                    break;
                }
            case DocumentTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateDocumentMessage);
                    break;
                }
            case TaskTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateTaskMessage);
                    break;
                }
            case SwcrOtherReferencesTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateSwcrOtherReferenceMessage);
                    break;
                }
            case SwcrTypeTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateSwcrTypeMessage);
                    break;
                }
            case SwcrAttachmentTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateSwcrAttachmentMessage);
                    break;
                }
            case TagEquipmentTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateTagEquipmentMessage);
                    break;
                }
            case ActionTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateActionMessage);
                    break;
                }
            case CommPkgTaskTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateCommPkgTaskMessage);
                    break;
                }
            case "mcpkgmilestone":
                {
                    await CreateAndSetMessage(busEvent, _service.CreateMcPkgMilestoneMessage);
                    break;
                }
            case "commpkgmilestone":
                {
                    await CreateAndSetMessage(busEvent, _service.CreateCommPkgMilestoneMessage);
                    break;
                }
            case WorkOrderTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateWorkOrderMessage);
                    break;
                }
            case WorkOrderCutoffTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateWorkOrderCutoffMessage);
                    break;
                }
            case CallOffTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateCallOffMessage);
                    break;
                }
            case WoChecklistTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateWorkOrderChecklistMessage);
                    break;
                }
            case WoMilestoneTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateWorkOrderMilestoneMessage);
                    break;
                }
            case SwcrTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateSwcrMessage);
                    break;
                }
            case SwcrSignatureTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateSwcrSignatureMessage);
                    break;
                }
            case LoopContentTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateLoopContentMessage);
                    break;
                }
            case LibraryFieldTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateLibraryFieldMessage);
                    break;
                }
            case StockTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateStockMessage);
                    break;
                }
            case PipingRevisionTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreatePipingRevisionMessage);
                    break;
                }
            case PipingSpoolTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreatePipingSpoolMessage);
                    break;
                }
            case WoMaterialTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateWorkOrderMaterialMessage);
                    break;
                }
            case "heattracepipetest":
                {
                    await CreateAndSetMessage(busEvent, _service.CreateHeatTracePipeTestMessage);
                    break;
                }
            case ProjectTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateProjectMessage);
                    break;
                }
            case PunchListItemTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreatePunchListItemMessage);
                    break;
                }
            case "notification":
                {
                    await CreateAndSetMessage(busEvent, _service.CreateNotificationMessage);
                    break;
                }
            case "notificationworkorder":
                {
                    await CreateAndSetMessage(busEvent, _service.CreateNotificationWorkOrderMessage);
                    break;
                }
            case "notificationcommpkg":
                {
                    await CreateAndSetMessage(busEvent, _service.CreateNotificationCommPkgMessage);
                    break;
                }
            case "notificationsignature":
                {
                    await CreateAndSetMessage(busEvent, _service.CreateNotificationSignatureMessage);
                    break;
                }
            case "punchprioritylibraryrelation":
                {
                    await CreateAndSetMessage(busEvent, _service.CreatePunchPriorityLibRelationMessage);
                    break;
                }
        }

        _logger.LogDebug("Update for  {Event} took {Ms} ms", busEvent.Event, sw.ElapsedMilliseconds);
        sw.Stop();
    }
}
