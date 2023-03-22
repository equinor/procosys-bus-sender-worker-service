﻿using System.Collections.Generic;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;

public interface ITelemetryClient
{
    void TrackMetric(string name, double metric);
    void TrackMetric(string name, double metric, string dimension1Name, string dimension2Name, string? dimension1Value, string dimension2Value);
    void Flush();
}
