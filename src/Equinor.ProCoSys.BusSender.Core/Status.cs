namespace Equinor.ProCoSys.BusSenderWorker.Core;

public enum Status
{
    UnProcessed = 0,
    Processing = 1,
    Failed = 2,
    Sent = 3,
    Skipped = 4,
    NotFound = 5
}
