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
    private readonly IConfiguration _configuration;
    private readonly int _queueWriteIntervalMinutes;


    public QueueMonitorService(ITelemetryClient telemetryClient, IBusEventRepository busEventRepository, IConfiguration configuration)
    {
        _telemetryClient = telemetryClient;
        _busEventRepository = busEventRepository;
        _configuration = configuration;
        _queueWriteIntervalMinutes = configuration.GetValue("MonitorQueueIntervalMinutes", 15);
    }

    public async Task WriteQueueMetrics()
    {
        var lastQueueWrite = _configuration.GetValue<DateTime>("LastQueueWrite", default);
        if (IsTimeToWriteQueueMetric(lastQueueWrite))
        {
            await WriteQueueLength();
            await WriteQueueAge();
            _configuration["LastQueueWrite"] = TimeService.UtcNow.ToString("O");
        }
    }

    private async Task WriteQueueAge()
    {
        var queueOldestEvent = await _busEventRepository.GetOldestEvent();
        
        if(NoEventFound(queueOldestEvent))
        {
            queueOldestEvent = TimeService.UtcNow;
        }      

        var waitTime = TimeService.UtcNow - queueOldestEvent;
        _telemetryClient.TrackMetric("QueueAge", waitTime.TotalMinutes);
    }

    private async Task WriteQueueLength()
    {
        var queueLength = await _busEventRepository.GetUnProcessedCount();
        _telemetryClient.TrackMetric("QueueLength", queueLength);
    }

    private bool IsTimeToWriteQueueMetric(DateTime lastQueueWrite) =>
        TimeService.UtcNow >= lastQueueWrite.ToUniversalTime().AddMinutes(_queueWriteIntervalMinutes);

    private bool NoEventFound(DateTime oldestEvent) => 
        oldestEvent.Equals(default);
}
