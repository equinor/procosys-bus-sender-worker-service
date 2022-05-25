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

    public async Task SendMessageChunk()
    {
        try
        {
            _logger.LogInformation($"BusSenderService SendMessageChunk starting at: {DateTimeOffset.Now}");
            var events = await _busEventRepository.GetEarliestUnProcessedEventChunk();
            if (events.Any())
            {
                _telemetryClient.TrackMetric("BusSender Chunk", events.Count);
            }

            _logger.LogInformation($"BusSenderService found {events.Count} messages to process");


            foreach (var busEvent in events)
            {

                if (busEvent.Event == TagTopic.TopicName)
                {
                    busEvent.Message = await _service.AttachTagDetails(busEvent.Message);
                }

                if (busEvent.Event is "query")
                {

                    busEvent.Message = await _service.CreateQueryMessage(busEvent.Message);
                }

                if (busEvent.Event is "document")
                {
                    busEvent.Message = await _service.CreateDocumentMessage(busEvent.Message);
                }


                /***
                 * WO_MATERIAL gets several inserts when saving a material, resulting in multiple rows in the BUSEVENT table.
                 * here we filter out all but the latest material event for a records with the same id and set those to Status = Skipped.
                 * This is to reduce spam on the bus.
                 */
                if (busEvent.Event == WoMaterialTopic.TopicName && _service.IsNotLatestMaterialEvent(events, busEvent))
                {
                    busEvent.Sent = Status.Skipped;
                    await _unitOfWork.SaveChangesAsync();
                    continue;
                }

                var message = JsonSerializer.Deserialize<BusEventMessage>(_service.WashString(busEvent.Message));

                if (message != null && string.IsNullOrEmpty(message.ProjectName))
                {
                    message.ProjectName = "_";
                }
                TrackMetric(message);
                await _topicClients.SendAsync(busEvent.Event, _service.WashString(busEvent.Message));

                TrackEvent(busEvent.Event, message);
                busEvent.Sent = Status.Sent;
                await _unitOfWork.SaveChangesAsync();
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"BusSenderService execute send failed at: {DateTimeOffset.Now}");
            throw;
        }
        _logger.LogInformation($"BusSenderService SendMessageChunk finished at: {DateTimeOffset.Now}");
        _telemetryClient.Flush();
    }

    private void TrackMetric(BusEventMessage message) =>
        _telemetryClient.TrackMetric("BusSender Topic", 1, "Plant", "ProjectName", message.Plant[4..],
            message.ProjectName?.Replace('$', '_') ?? "NoProject");

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

    public async Task StopService()
    {
        _logger.LogInformation($"BusSenderService stop reader at: {DateTimeOffset.Now}");
        await _topicClients.CloseAllAsync();
    }
}
