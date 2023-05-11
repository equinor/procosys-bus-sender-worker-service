namespace Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;

public interface ITelemetryClient
{
    void Flush();
    void TrackMetric(string name, double metric);

    void TrackMetric(string name, double metric, string dimension1Name, string dimension2Name, string? dimension1Value,
        string dimension2Value);
}
