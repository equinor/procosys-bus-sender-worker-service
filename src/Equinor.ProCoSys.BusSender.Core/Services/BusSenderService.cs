using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;
using Equinor.ProCoSys.BusSenderWorker.Core;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;
using Microsoft.Azure.ServiceBus;

namespace Equinor.ProCoSys.BusSender.Core.Services
{
    public class BusSenderService : IBusSenderService
    {
        private readonly IPcsBusSender _topicClients;
        private readonly IBusEventRepository _busEventRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BusSenderService> _logger;
        private readonly ITelemetryClient _telemetryClient;
        private readonly Regex _rx = new Regex(@"[\a\e\f\n\r\t\v]", RegexOptions.Compiled);


        public BusSenderService(IPcsBusSender topicClients,
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
                _logger.LogInformation($"BusSenderService SendMessageChunk starting at: {DateTimeOffset.Now}");
                var events = await _busEventRepository.GetEarliestUnProcessedEventChunk();
                if (events.Any())
                {
                    _telemetryClient.TrackMetric("BusSender Chunk", events.Count);
                }

                _logger.LogInformation($"BusSenderService found {events.Count} messages to process");
                foreach (var busEvent in events)
                {
                    var message = JsonSerializer.Deserialize<BusEventMessage>(WashString(busEvent.Message));
                    TrackMetric(message);
                    await _topicClients.SendAsync(busEvent.Event, WashString(busEvent.Message));
                    
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
            _telemetryClient.TrackMetric("BusSender Topic", 1, "ProjectSchema", "ProjectName", message.ProjectSchema[4..],
                message.ProjectName.Replace('$', '_'));

        private void TrackEvent(string eventType, BusEventMessage message)
        {
            var properties = new Dictionary<string, string>
            {
                {"Event", eventType},
                {"ProjectSchema", message.ProjectSchema[4..]},
                {"ProjectName", message.ProjectName.Replace('$', '_')}
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
        
        private string WashString(string busEventMessage)
        {
            busEventMessage = _rx.Replace(busEventMessage, m => Regex.Escape(m.Value));

            return busEventMessage;
        }
    }
}
