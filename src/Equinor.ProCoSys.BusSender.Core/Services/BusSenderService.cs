using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class BusSenderService : IBusSenderService
{
    private readonly IPcsBusSender _topicClients;
    private readonly IBusEventRepository _busEventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BusSenderService> _logger;
    private readonly ITelemetryClient _telemetryClient;
    private readonly IBusEventService _service;
    private readonly Stopwatch _sw;

    public BusSenderService(IPcsBusSender topicClients,
        IBusEventRepository busEventRepository,
        IUnitOfWork unitOfWork,
        ILogger<BusSenderService> logger,
        ITelemetryClient telemetryClient,
        IBusEventService service)
    {
        _topicClients = topicClients;
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
            _logger.LogDebug("BusSenderService HandleBusEvents starting");
            var events = await _busEventRepository.GetEarliestUnProcessedEventChunk();
            if (events.Any())
            {
                _telemetryClient.TrackMetric("BusSender Chunk", events.Count);
                _logger.LogInformation("BusSenderService found {count} messages to process after {sw} ms", events.Count, _sw.ElapsedMilliseconds);

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

        _logger.LogInformation("BusSenderService DoWorkerJob finished ");
        _telemetryClient.Flush();
    }

    public async Task CloseConnections()
    {
        _logger.LogInformation("BusSenderService stop reader and close all connections");
        await _topicClients.CloseAllAsync();
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
    private static IEnumerable<IGrouping<(string, long), BusEvent>> FilterOnSimpleMessagesAndGroupDuplicates(
        IEnumerable<BusEvent> events)
        => events.Where(e => long.TryParse(e.Message, out var id))
            .GroupBy(e => (e.Event, long.Parse(e.Message)));

    private async Task ProcessBusEvents(List<BusEvent> events)
    {
        events = SetDuplicatesToSkipped(events);
        _logger.LogInformation("SetDuplicatesToSkipped after {sw} ms", _sw.ElapsedMilliseconds);

        var listOfTasks = events.Where(busEvent => busEvent.Status == Status.UnProcessed)
            .Select(UpdateEventBasedOnTopic)
            .Cast<Task>()
            .ToList();

        await Task.WhenAll(listOfTasks);
         _logger.LogDebug("Updated events at {sw} ms",  _sw.ElapsedMilliseconds);

         if (HasUnsavedChanges(events))
         {
             await _unitOfWork.SaveChangesAsync();
             _logger.LogDebug("HasUnsavedChanges after {sw} ms",  _sw.ElapsedMilliseconds);
        }

        foreach (var busEvent in events.Where(busEvent => busEvent.Status == Status.UnProcessed))
        {

            // var handledEvent = await UpdateEventBasedOnTopic(busEvent);
            // _logger.LogDebug("Update event {event} at {sw} ms", busEvent.Event, _sw.ElapsedMilliseconds);


            var message =
                JsonSerializer.Deserialize<BusEventMessage>(_service.WashString(busEvent.Message));

            if (message != null && string.IsNullOrEmpty(message.ProjectName))
            {
                message.ProjectName = "_";
            }

            TrackMetric(message);
            await _topicClients.SendAsync(busEvent.Event, _service.WashString(busEvent.Message));

            TrackEvent(busEvent.Event, message);
            busEvent.Status = Status.Sent;
            await _unitOfWork.SaveChangesAsync();
        }
    }

    private static bool HasUnsavedChanges(IEnumerable<BusEvent> events) => events.Any(e => e.Status != Status.UnProcessed);

    private async Task<BusEvent> UpdateEventBasedOnTopic(BusEvent busEvent)
    {
        switch (busEvent.Event)
        {
            case TagTopic.TopicName:
                {
                    busEvent.Message = await _service.AttachTagDetails(busEvent.Message);
                    break;
                }
            case ChecklistTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateChecklistMessage);
                    break;
                }
            case CommPkgQueryTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateCommPkgQueryMessage);
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
            case MilestoneTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateMilestoneMessage);
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
        return busEvent;
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
            busEvent.Message = message;
        }
    }

    private void TrackMetric(BusEventMessage message) =>
        _telemetryClient.TrackMetric("BusSender Topic", 1, "Plant", "ProjectName", message?.Plant?[4..],
            message?.ProjectName?.Replace('$', '_') ?? "NoProject");

    private void TrackEvent(string eventType, BusEventMessage message)
    {
        var properties = new Dictionary<string, string>
        {
            { "Event", eventType },
            { "Plant", message.Plant[4..] },
            { "ProjectName", message.ProjectName?.Replace('$', '_') }
        };
        if (!string.IsNullOrWhiteSpace(message.McPkgNo))
        {
            properties.Add("McPkgNo", message.McPkgNo);
        }

        if (!string.IsNullOrWhiteSpace(message.CommPkgNo))
        {
            properties.Add("CommPkgNo", message.CommPkgNo);
        }

        _telemetryClient.TrackEvent("BusSender Send",
            properties);
    }
}
