using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class HeatTraceEvent : IHeatTraceEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public long HeatTraceId { get; set; }
    public long CableId { get; set; }
    public Guid CableGuid { get; set; }
    public string CableNo { get; set; }
    public long TagId { get; set; }
    public Guid TagGuid { get; set; }
    public string TagNo { get; set; }
    public string? SpoolNo { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.HeatTraceCreateOrUpdate;
}
