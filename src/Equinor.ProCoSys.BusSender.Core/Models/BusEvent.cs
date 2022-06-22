using System;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class BusEvent
{
    public long Id { get; set; }
    public string Event { get; set; }
    public string Message { get; set; }
    public DateTime Created { get; set; }
    public Status Status { get; set; }
}
