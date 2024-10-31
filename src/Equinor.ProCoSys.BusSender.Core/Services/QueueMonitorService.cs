using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.Common.Time;
using Microsoft.Extensions.Configuration;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class QueueMonitorService : IQueueMonitorService
{
    private readonly ITelemetryClient _telemetryClient;
    private readonly IBusEventRepository _busEventRepository;
    private DateTime _lastQueueWrite = DateTime.MinValue;
    private readonly int _queueWriteIntervalMinutes;


    public QueueMonitorService(ITelemetryClient telemetryClient, IBusEventRepository busEventRepository, IConfiguration configuration)
    {
        _telemetryClient = telemetryClient;
        _busEventRepository = busEventRepository;
        _queueWriteIntervalMinutes = configuration.GetValue("QueueWriteIntervalMinutes", 15);
    }

    public async Task WriteQueueMetrics()
    {
        if (ShouldWriteQueueMetrics())
        {
            await WriteQueueLength();
            await WriteWaitTime();
            _lastQueueWrite = TimeService.UtcNow;
        }
    }

    private async Task WriteWaitTime()
    {
        var queueOldestEvent = await _busEventRepository.GetOldestEvent();
        
        if(NoEventFound(queueOldestEvent))
        {
            queueOldestEvent = TimeService.UtcNow;
        }      

        var waitTime = TimeService.UtcNow - queueOldestEvent;
        _telemetryClient.TrackMetric("QueueAge", waitTime.Minutes);
    }

    private async Task WriteQueueLength()
    {
        var queueLength = await _busEventRepository.GetUnProcessedCount();
        _telemetryClient.TrackMetric("QueueLength", queueLength);
    }

    private bool ShouldWriteQueueMetrics() =>
        IsFirstQueueWrite() || IsTimeToWriteQueueMetric();

    private bool IsTimeToWriteQueueMetric() =>
        TimeService.UtcNow >= _lastQueueWrite.AddMinutes(_queueWriteIntervalMinutes);

    private bool IsFirstQueueWrite() =>
        _lastQueueWrite.Equals(DateTime.MinValue);

    private static bool NoEventFound(DateTime oldestEvent) => 
        oldestEvent.Equals(default);
}
