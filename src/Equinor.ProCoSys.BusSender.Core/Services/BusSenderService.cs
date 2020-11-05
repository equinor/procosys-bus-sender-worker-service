﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Core.Interfaces;
using Equinor.ProCoSys.BusSender.Core.Telemetry;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSender.Core.Services
{
    public class BusSenderService : IBusSenderService
    {
        private readonly ITopicClients _topicClients;
        private readonly IBusEventRepository _busEventRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BusSenderService> _logger;
        private readonly ITelemetryClient _telemetryClient;

        public BusSenderService(ITopicClients topicClients,
            IBusEventRepository busEventRepository,
            IUnitOfWork unitOfWork,
            ILogger<BusSenderService> logger,
            ITelemetryClient telemetryClient)
        {
            _topicClients = topicClients;
            _busEventRepository = busEventRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _telemetryClient = telemetryClient;
        }

        public async Task SendMessageChunk()
        {
            try
            {
                _logger.LogDebug($"BusSenderService SendMessageChunk starting at: {DateTimeOffset.Now}");
                var events = await _busEventRepository.GetEarliestUnProcessedEventChunk();
                if (events.Any())
                {
                    _telemetryClient.TrackMetric("BusSender Chunk", events.Count);
                }

                _logger.LogInformation($"BusSenderService found {events.Count} messages to process");
                foreach (var busEvent in events)
                {
                    _logger.LogTrace($"Sending: {busEvent.Message}");
                    await _topicClients.Send(busEvent.Event, busEvent.Message);
                    _telemetryClient.TrackEvent("BusSender Send",
                        new Dictionary<string, string>
                        {
                            {"Event", busEvent.Event},
                        });
                    busEvent.Sent = Status.Sent;
                    await _unitOfWork.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"BusSenderService execute send failed at: {DateTimeOffset.Now}");
                throw;
            }
            _logger.LogDebug($"BusSenderService SendMessageChunk finished at: {DateTimeOffset.Now}");
        }

        public async Task StopService()
        {
            _logger.LogInformation($"BusSenderService stop reader at: {DateTimeOffset.Now}");
            await _topicClients.CloseAllAsync();
        }
    }
}
