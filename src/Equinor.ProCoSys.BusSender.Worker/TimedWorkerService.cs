using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSender.Worker;

public class TimedWorkerService : IHostedService, IDisposable
{
    private readonly IEntryPointService _entryPointService;
    private readonly ILogger<TimedWorkerService> _logger;
    private readonly int _timeout;
    private Timer? _timer;

    public TimedWorkerService(ILogger<TimedWorkerService> logger, IEntryPointService entryPointService,
        IConfiguration configuration)
    {
        _logger = logger;
        _entryPointService = entryPointService;
        _timeout = int.Parse(configuration["TimerInterval"] ?? "1000");
    }

    public void Dispose()
    {
        _timer?.Dispose();
        GC.SuppressFinalize(this);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"TimedWorkerService started at: {DateTimeOffset.Now}");

        _timer = new Timer(DoWork, null, 5000, Timeout.Infinite);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation($"TimedWorkerService stopping at: {DateTimeOffset.Now}");
        _timer?.Change(Timeout.Infinite, 0);
        _entryPointService.StopService();
        return Task.CompletedTask;
    }

    private async void DoWork(object? state)
    {
        try
        {
            _logger.LogInformation("TimedWorkerService started do work");
            await _entryPointService.DoWorkerJob();
            _logger.LogInformation("TimedWorkerService finished do work");
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"TimedWorker service failed in do work at: {DateTimeOffset.Now}");
        }
        finally
        {
            _timer?.Change(_timeout, Timeout.Infinite);
        }
    }
}
