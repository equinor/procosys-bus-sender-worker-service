using System;
#pragma warning disable CS8618

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class BusEvent
{
    public long Id { get; set; }
    public string Event { get; set; }
    public string Message { get; set; }
    public DateTime Created { get; init; }
    public Status Status { get; set; }

    //untracked
    public string? MessageToSend { get; set; }
}
