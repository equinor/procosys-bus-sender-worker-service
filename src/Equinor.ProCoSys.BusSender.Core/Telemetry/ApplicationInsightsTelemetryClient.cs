﻿using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;

public class ApplicationInsightsTelemetryClient : ITelemetryClient
{
    private readonly TelemetryClient _ai;

    public ApplicationInsightsTelemetryClient(TelemetryConfiguration telemetryConfiguration)
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
    }

    public void Flush() => _ai.Flush();

    public void TrackMetric(string name, double metric) =>
        _ai
            .GetMetric(name)
            .TrackValue(metric);

    public void TrackMetric(string name, double metric, string dimension1Name, string dimension2Name,
        string? dimension1Value, string dimension2Value) =>
        _ai
            .GetMetric(name, dimension1Name, dimension2Name)
            .TrackValue(metric, dimension1Value, dimension2Value);

    public void TrackEvent(string name, Dictionary<string, string> properties) =>
        _ai
            .TrackEvent(name, properties);

    public void TrackMetric(string name, double metric, string dimension1Name, string dimension1Value) =>
        _ai
            .GetMetric(name, dimension1Name)
            .TrackValue(metric, dimension1Value);
}
