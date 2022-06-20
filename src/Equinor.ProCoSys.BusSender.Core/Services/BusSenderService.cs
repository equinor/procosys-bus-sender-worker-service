using System;
using System.Collections.Generic;
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
    }

    public async Task HandleBusEvents()
    {
        try
        {
            _logger.LogInformation($"BusSenderService DoWorkerJob starting at: {DateTimeOffset.Now}");
            var events = await _busEventRepository.GetEarliestUnProcessedEventChunk();
            if (events.Any())
            {
                _telemetryClient.TrackMetric("BusSender Chunk", events.Count);
            }
            _logger.LogInformation("BusSenderService found {count} messages to process",events.Count);
            await ProcessBusEvents(events);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"BusSenderService execute send failed at: {DateTimeOffset.Now}");
            throw;
        }
        _logger.LogInformation($"BusSenderService DoWorkerJob finished at: {DateTimeOffset.Now}");
        _telemetryClient.Flush();
    }

    private static List<BusEvent> SetDuplicatesToSkipped(List<BusEvent> events)
    {
        var map = events.Where(e => long.TryParse(e.Message, out var id)).GroupBy(e => new { e.Event, id = long.Parse(e.Message)});
        foreach (var group in map)
        {
            foreach (var busEvent in group.SkipLast(1))
            {
                busEvent.Status = Status.Skipped;
            }
        }

        return events;
    }

    private async Task ProcessBusEvents(List<BusEvent> events)
    {
        events = SetDuplicatesToSkipped(events);

        if (events.Any(e => e.Status != Status.UnProcessed))
        {
            await _unitOfWork.SaveChangesAsync();
        }

        foreach (var busEvent in events.Where(busEvent => busEvent.Status == Status.UnProcessed))
        {
            var handledEvent = await UpdateEventBasedOnTopic(events, busEvent);

            if (busEvent.Status != Status.UnProcessed)
            {
                continue;
            }

            var message =
                JsonSerializer.Deserialize<BusEventMessage>(_service.WashString(handledEvent.Message));

            if (message != null && string.IsNullOrEmpty(message.ProjectName))
            {
                message.ProjectName = "_";
            }

            TrackMetric(message);
            await _topicClients.SendAsync(busEvent.Event, _service.WashString(handledEvent.Message));

            TrackEvent(handledEvent.Event, message);
            busEvent.Status = Status.Sent;
            await _unitOfWork.SaveChangesAsync();
        }
        
    }

    private async Task<BusEvent> UpdateEventBasedOnTopic(IEnumerable<BusEvent> events, BusEvent busEvent)
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
                    var checklistMessage = await _service.CreateChecklistMessage(busEvent.Message);
                    if (checklistMessage == null)
                    {
                        busEvent.Status = Status.NotFound;
                        return busEvent;
                    }

                    busEvent.Message = checklistMessage;
                    break;
                }
            case QueryTopic.TopicName:
                {
                    var queryMessage = await _service.CreateQueryMessage(busEvent.Message);
                    if (queryMessage == null)
                    {
                        busEvent.Status = Status.NotFound;
                        return busEvent;
                    }

                    busEvent.Message = queryMessage;
                    break;
                }
            case DocumentTopic.TopicName:
                {
                    var documentMessage = await _service.CreateDocumentMessage(busEvent.Message);
                    if (documentMessage == null)
                    {
                        busEvent.Status = Status.NotFound;
                        return busEvent;
                    }
                    busEvent.Message = documentMessage;
                    break;
                }
            case WorkOrderTopic.TopicName:
                {
                    var workOrderMessage = await _service.CreateWorkOrderMessage(busEvent.Message);
                    if (workOrderMessage == null)
                    {
                        busEvent.Status = Status.NotFound;
                        return busEvent;
                    }
                    busEvent.Message = workOrderMessage;


                    break;
                }
                /***
                 * WO_MATERIAL gets several inserts when saving a material, resulting in multiple rows in the BUSEVENT table.
                 * here we filter out all but the latest material event for a records with the same id and set those to Status = Skipped.
                 * This is to reduce spam on the bus.
                 */
            case WoMaterialTopic.TopicName when _service.IsNotLatestMaterialEvent(events, busEvent):
                {
                    busEvent.Status = Status.Skipped;
                    return busEvent;
                }
        }

        return busEvent;
    }

    private void TrackMetric(BusEventMessage message) =>
        _telemetryClient.TrackMetric("BusSender Topic", 1, "Plant", "ProjectName", message?.Plant?[4..],
            message?.ProjectName?.Replace('$', '_') ?? "NoProject");

    private void TrackEvent(string eventType, BusEventMessage message)
    {
        var properties = new Dictionary<string, string>
        {
            {"Event", eventType},
            {"Plant", message.Plant[4..]},
            {"ProjectName", message.ProjectName?.Replace('$', '_')}
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

    public async Task CloseConnections()
    {
        _logger.LogInformation($"BusSenderService stop reader at: {DateTimeOffset.Now}");
        await _topicClients.CloseAllAsync();
    }
}
