namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class BusEventMessage
{
    public string? Plant { get; set; }
    public string? ProCoSysGuid { get; set; }
    public string? ProjectName { get; set; }

}
