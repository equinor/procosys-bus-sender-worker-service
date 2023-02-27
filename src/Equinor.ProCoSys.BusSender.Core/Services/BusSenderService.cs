using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.BusSenderWorker.Core.Extensions;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Mappers;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class BusSenderService : IBusSenderService
{
    private readonly IPcsBusSender _pcsBusSender;
    private readonly IBusEventRepository _busEventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BusSenderService> _logger;
    private readonly ITelemetryClient _telemetryClient;
    private readonly IBusEventService _service;
    private readonly Stopwatch _sw;

    public BusSenderService(IPcsBusSender pcsBusSender,
        IBusEventRepository busEventRepository,
        IUnitOfWork unitOfWork,
        ILogger<BusSenderService> logger,
        ITelemetryClient telemetryClient,
        IBusEventService service)
    {
        _pcsBusSender = pcsBusSender;
        _busEventRepository = busEventRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _telemetryClient = telemetryClient;
        _service = service;
        _sw = new Stopwatch();
    }

    public async Task HandleBusEvents()
    {
        try
        {
            _sw.Start();
            var events = await _busEventRepository.GetEarliestUnProcessedEventChunk();
            if (events.Any())
            {
                _logger.LogInformation("BusSenderService found {count} messages to process after {sw} ms", events.Count, _sw.ElapsedMilliseconds);
                _telemetryClient.TrackMetric("BusSender Chunk", events.Count);
                await ProcessBusEvents(events);
                _logger.LogInformation("BusSenderService ProcessBusEvents used {sw} ms", _sw.ElapsedMilliseconds);

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

    public async Task CloseConnections()
    {
        _logger.LogInformation("BusSenderService stop reader and close all connections");
        await _pcsBusSender.CloseAllAsync();
    }

    private async Task ProcessBusEvents(List<BusEvent> events)
    {
        events = SetDuplicatesToSkipped(events);
        var dsw = Stopwatch.StartNew();

        var unProcessedEvents = events.Where(busEvent => busEvent.Status == Status.UnProcessed).ToList();
        _logger.LogInformation("Amount of messages to process: {count} ", unProcessedEvents.Count);

        foreach (var e in unProcessedEvents.Where( e => IsSimpleMessage(e) || e.Event == TagTopic.TopicName))
        {
            await UpdateEventBasedOnTopic(e);
        }
        _logger.LogInformation("Update loop finished at at {sw} ms", dsw.ElapsedMilliseconds);
        await _unitOfWork.SaveChangesAsync();
        
        
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
                if (messageBatch.TryAddMessage(
                        new ServiceBusMessage(
                            _service.WashString(messages.Peek().MessageToSend ?? messages.Peek().Message))))
                {
                    var m = messages.Dequeue();
                    m.Status = Status.Sent;
                    TrackMessage(m);
                }
                else
                {
                    // if the first message can't fit, then it is too large for the batch
                    throw new Exception($"Message {messageCount - messages.Count} is too large and cannot be sent.");
                }

                // add as many messages as possible to the current batch
                while (messages.Count > 0 &&
                       messageBatch.TryAddMessage(
                           new ServiceBusMessage(
                               _service.WashString(messages.Peek().MessageToSend ?? messages.Peek().Message))))
                {
                    var m = messages.Dequeue();
                    m.Status = Status.Sent;
                    TrackMessage(m);
                }

                _logger.LogDebug("Sending amount: {count} after {ms} ms", messageBatch.Count, _sw.ElapsedMilliseconds);
                await _pcsBusSender.SendMessagesAsync(messageBatch, topic);
                await _unitOfWork.SaveChangesAsync();
                _logger.LogDebug("done sending and save after {ms} ms", _sw.ElapsedMilliseconds);
            }
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
    private static void SetAllButOneEventToSkipped(IEnumerable<BusEvent> group)
    {
        foreach (var busEvent in group.SkipLast(1))
        {
            busEvent.Status = Status.Skipped;
        }
    }

    /// <summary>
    ///     Removes all events that have Message = more than just a simple Id that can be converted to a long.
    ///     Then groups the events into bulks based on Id and Event type.
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    private static IEnumerable<IGrouping<(string, string), BusEvent>> FilterOnSimpleMessagesAndGroupDuplicates(
        IEnumerable<BusEvent> events)
        => events.Where(IsSimpleMessage)
            .GroupBy(e => (e.Event, e.Message));

    private static bool IsSimpleMessage(BusEvent e) 
        => long.TryParse(e.Message, out _) 
           || Guid.TryParse(e.Message, out _)
           || BusEventService.CanGetTwoIdsFromMessage(e.Message.Split(","),out _,out _);


    private void TrackMessage(BusEvent busEvent)
    {
        var busEventMessageToSend = busEvent.MessageToSend ?? busEvent.Message;
        var message = JsonSerializer.Deserialize<BusEventMessage>(_service.WashString(busEventMessageToSend), DefaultSerializerHelper.SerializerOptions);
        if (message != null && string.IsNullOrEmpty(message.ProjectName))
        {
            message.ProjectName = "_";
        }
        TrackMetric(message);
    }

    private async Task UpdateEventBasedOnTopic(BusEvent busEvent)
    {
        _logger.LogDebug("Started update for {event}", busEvent.Event);
        var sw = Stopwatch.StartNew();
        switch (busEvent.Event)
        {
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
            case PcsTopicConstants.McPkgMilestone:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateMcPkgMilestoneMessage);
                    break;
                }
            case PcsTopicConstants.CommPkgMilestone:
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
                    await CreateAndSetMessage(busEvent, _service.CreateWorkOrderCutOffMessage);
                    break;
                }
            case CallOffTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateCallOffMessage);
                    break;
                }
            case WoChecklistTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateWoChecklistMessage);
                    break;
                }
            case WoMilestoneTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateWoMilestoneMessage);
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
                    await CreateAndSetMessage(busEvent, _service.CreateWoMaterialMessage);
                    break;
                }
        }
        _logger.LogDebug("Update for  {event} took {ms} ms", busEvent.Event, sw.ElapsedMilliseconds);
        sw.Stop();
    }

    /***
     * Takes a function to create a message, caller needs to make sure that event has the correct topic for the function.
     */
    private static async Task CreateAndSetMessage(BusEvent busEvent, Func<string, Task<string>> createMessageFunction)
    {
        var message = await createMessageFunction(busEvent.Message);
        if (message == null)
        {
            busEvent.Status = Status.NotFound;
        }
        else
        {
            busEvent.MessageToSend = message;
        }
    }

    private void TrackMetric(BusEventMessage message) =>
        _telemetryClient.TrackMetric("BusSender Topic", 1, "Plant", "ProjectName", message?.Plant?[4..],
            message?.ProjectName?.Replace('$', '_') ?? "NoProject");
}
