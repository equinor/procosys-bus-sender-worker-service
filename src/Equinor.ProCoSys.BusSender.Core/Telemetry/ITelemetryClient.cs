using System.Collections.Generic;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;

public interface ITelemetryClient
{
    void Flush();
    void TrackMetric(string name, double metric);
    
    void TrackEvent(string name, Dictionary<string, string> properties);    
    
}
