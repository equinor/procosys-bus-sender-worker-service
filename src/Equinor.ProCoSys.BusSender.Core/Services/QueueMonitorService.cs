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
        _queueWriteIntervalMinutes = string.IsNullOrWhiteSpace(configuration["MonitorQueueIntervalMinutes"]) ? 15 : int.Parse(configuration["MonitorQueueIntervalMinutes"]!);
    }

    public async Task WriteQueueMetrics(string? plant = null)
    {
        var lastQueueWrite = string.IsNullOrWhiteSpace(_configuration["LastQueueWrite"]) ? default : DateTime.Parse(_configuration["LastQueueWrite"]!);
        if (IsTimeToWriteQueueMetric(lastQueueWrite))
        {
            await WriteQueueLength(plant);
            await WriteQueueAge(plant);
            _configuration["LastQueueWrite"] = _systemClock.UtcNow.ToString("O");
        }
    }

    private async Task WriteQueueAge(string? plant = null)
    {
        var queueOldestEvent = await _busEventRepository.GetOldestEvent(ignoreFilter: true);

        if (NoEventFound(queueOldestEvent))
        {
            queueOldestEvent = _systemClock.UtcNow;
        }

        var waitTime = _systemClock.UtcNow - queueOldestEvent;
        _telemetryClient.TrackMetric("QueueAge", waitTime.TotalMinutes);
        if (plant != null)
        {
            queueOldestEvent = await _busEventRepository.GetOldestEvent(ignoreFilter: false);

            if (NoEventFound(queueOldestEvent))
            {
                queueOldestEvent = _systemClock.UtcNow;
            }

            waitTime = _systemClock.UtcNow - queueOldestEvent;
            _telemetryClient.TrackMetric($"{plant ?? ""}QueueAge", waitTime.TotalMinutes);
        }
    }

    private async Task WriteQueueLength(string? plant = null)
    {
        var queueLength = await _busEventRepository.GetUnProcessedCount(ignoreFilter: true);
        _telemetryClient.TrackMetric("QueueLength", queueLength);
        if (plant != null)
        {
            queueLength = await _busEventRepository.GetUnProcessedCount(ignoreFilter: false);
            _telemetryClient.TrackMetric($"{plant ?? ""}QueueLength", queueLength);
        }
    }

    private bool IsTimeToWriteQueueMetric(DateTime lastQueueWrite) =>
        _systemClock.UtcNow >= lastQueueWrite.ToUniversalTime().AddMinutes(_queueWriteIntervalMinutes);

    private static bool NoEventFound(DateTime oldestEvent) => 
        oldestEvent.Equals(default);
}
