using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSender.Worker
{
    public class TimedWorkerService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedWorkerService> _logger;
        private readonly IEntryPointService _entryPointService;
        private Timer _timer;
        private int _timeout;

        public TimedWorkerService(ILogger<TimedWorkerService> logger, IEntryPointService entryPointService, IConfiguration configuration)
        {
            _logger = logger;
            _entryPointService = entryPointService;
            _timeout = int.Parse(configuration["TimerInterval"]);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogWarning($"TimedWorkerService at: {DateTimeOffset.Now}");

            _timer = new Timer(DoWork, null, 5000, Timeout.Infinite );

            return Task.CompletedTask;
        }

        private async void DoWork(object state)
        {
            try
            {
                _logger.LogTrace($"TimedWorkerService started do work at: { DateTimeOffset.Now}");
                await _entryPointService.SendMessageChunk();
            }
            finally
            {
                _logger.LogTrace($"Resetting timer at: { DateTimeOffset.Now}");
                _timer.Change(_timeout, Timeout.Infinite );
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"TimedWorkerService stopping at: at: { DateTimeOffset.Now}");
            _timer?.Change(Timeout.Infinite, 0);
            _entryPointService.StopService();
            return Task.CompletedTask;
        }

        public void Dispose() => _timer?.Dispose();
    }
}
