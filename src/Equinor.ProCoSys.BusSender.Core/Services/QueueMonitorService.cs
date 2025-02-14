using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.PcsServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class QueueMonitorService : IQueueMonitorService
{
    private readonly ITelemetryClient _telemetryClient;
    private readonly IBusEventRepository _busEventRepository;
    private readonly IConfiguration _configuration;
    private readonly ISystemClock _systemClock;
    private readonly int _queueWriteIntervalMinutes;


    public QueueMonitorService(ITelemetryClient telemetryClient, IBusEventRepository busEventRepository, IConfiguration configuration, ISystemClock systemClock)
    {
        _telemetryClient = telemetryClient;
        _busEventRepository = busEventRepository;
        _configuration = configuration;
        _systemClock = systemClock;
        _queueWriteIntervalMinutes = string.IsNullOrWhiteSpace(configuration["MonitorQueueIntervalMinutes"]) ? 15 : int.Parse(configuration["MonitorQueueIntervalMinutes"]);
    }

    public async Task WriteQueueMetrics()
    {
        var lastQueueWrite = string.IsNullOrWhiteSpace(_configuration["LastQueueWrite"]) ? default : DateTime.Parse(_configuration["LastQueueWrite"]);
        if (IsTimeToWriteQueueMetric(lastQueueWrite))
        {
            await WriteQueueLength();
            await WriteQueueAge();
            _configuration["LastQueueWrite"] = _systemClock.UtcNow.ToString("O");
        }
    }

    private async Task WriteQueueAge()
    {
        var queueOldestEvent = await _busEventRepository.GetOldestEvent();
        
        if(NoEventFound(queueOldestEvent))
        {
            queueOldestEvent = _systemClock.UtcNow;
        }      

        var waitTime = _systemClock.UtcNow - queueOldestEvent;
        _telemetryClient.TrackMetric("QueueAge", waitTime.TotalMinutes);
    }

    private async Task WriteQueueLength()
    {
        var queueLength = await _busEventRepository.GetUnProcessedCount();
        _telemetryClient.TrackMetric("QueueLength", queueLength);
    }

    private bool IsTimeToWriteQueueMetric(DateTime lastQueueWrite) =>
        _systemClock.UtcNow >= lastQueueWrite.ToUniversalTime().AddMinutes(_queueWriteIntervalMinutes);

    private static bool NoEventFound(DateTime oldestEvent) => 
        oldestEvent.Equals(default);
}
