using System;
using System.Collections.Generic;
using Equinor.ProCoSys.PcsServiceBus;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;

public class RunningExecutableTelemetryInitializer : ITelemetryInitializer
{
    private readonly string _instanceName;

    public RunningExecutableTelemetryInitializer(string instanceName) => _instanceName = instanceName;

    public void Initialize(ITelemetry telemetry)
    {
        telemetry.Context.Cloud.RoleName = AppDomain.CurrentDomain.FriendlyName;
        telemetry.Context.GlobalProperties["InstanceName"] = _instanceName;
    }
}

public class ApplicationInsightsTelemetryClient : ITelemetryClient
{
    private readonly TelemetryClient _ai;

    public ApplicationInsightsTelemetryClient(TelemetryConfiguration telemetryConfiguration, IConfiguration configuration, IOptions<InstanceOptions> instanceOptions)
    {
        if (telemetryConfiguration == null)
        {
            throw new ArgumentNullException(nameof(telemetryConfiguration));
        }

        _ai = new TelemetryClient(telemetryConfiguration)
        {
            // The InstrumentationKey isn't set through the configuration object. Setting it explicitly works.
            TelemetryConfiguration = { ConnectionString = telemetryConfiguration.ConnectionString }
        };
        _ai.TelemetryConfiguration.TelemetryInitializers.Add(new RunningExecutableTelemetryInitializer(instanceOptions.Value.InstanceName));
    }

    public void Flush() => _ai.Flush();

    public void TrackEvent(string name, Dictionary<string, string> properties) =>
        _ai
            .TrackEvent(name, properties);

    public void TrackMetric(string name, double metric) =>
        _ai
            .GetMetric(name)
            .TrackValue(metric);

    public void TrackMetric(string name, double metric, Dictionary<string, string> properties) =>
        _ai
            .TrackMetric(name, metric, properties);
}
