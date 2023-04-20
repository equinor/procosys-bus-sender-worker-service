using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class McPkgEvent : IMcPkgEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string PlantName { get; set; }
    public string ProjectName { get; set; }
    public string McPkgNo { get; set; }
    public long McPkgId { get; set; }
    public string CommPkgNo { get; set; }
    public Guid CommPkgGuid { get; set; }
    public string? Description { get; set; }
    public string? Remark { get; set; }
    public string ResponsibleCode { get; set; }
    public string? ResponsibleDescription { get; set; }
    public string? AreaCode { get; set; }
    public string? AreaDescription { get; set; }
    public string Discipline { get; set; }
    public string McStatus { get; set; }
    public string? Phase { get; set; }
    public bool IsVoided { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.McPkgCreateOrUpdate;
}
