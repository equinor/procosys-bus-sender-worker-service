using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSender.Core.Services
{
    public class BusSenderService : IBusSenderService
    {
        private readonly ITopicClients _topicClients;
        private readonly IBusEventRepository _busEventRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<BusSenderService> _logger;

        public BusSenderService(ITopicClients topicClients,
            IBusEventRepository busEventRepository,
            IUnitOfWork unitOfWork,
            ILogger<BusSenderService> logger)
        {
            _topicClients = topicClients;
            _busEventRepository = busEventRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task SendMessageChunk()
        {
            try
            {
                _logger.LogDebug($"BusSenderService SendMessageChunk starting at: {DateTimeOffset.Now}");
                var events = await _busEventRepository.GetEarliestUnProcessedEventChunk();
                _logger.LogInformation($"BusSenderService found {events.Count} messages to process");
                foreach (var busEvent in events)
                {
                    _logger.LogTrace($"Sending: {busEvent.Message}");
                    await _topicClients.Send(busEvent.Event, busEvent.Message);
                    busEvent.Sent = 3;
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
