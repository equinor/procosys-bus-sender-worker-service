using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class CommPkgEvent : ICommPkgEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string PlantName { get; set; }
    public string ProjectName { get; set; }
    public string CommPkgNo { get; set; }
    public long CommPkgId { get; set; }
    public string Description { get; set; }
    public string CommPkgStatus { get; set; }
    public bool IsVoided { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? DescriptionOfWork { get; set; }
    public string? Remark { get; set; }
    public string ResponsibleCode { get; set; }
    public string? ResponsibleDescription { get; set; }
    public string? AreaCode { get; set; }
    public string? AreaDescription { get; set; }
    public string? Phase { get; set; }
    public string? CommissioningIdentifier { get; set; }
    public bool? Demolition { get; set; }
    public string? Priority1 { get; set; }
    public string? Priority2 { get; set; }
    public string? Priority3 { get; set; }
    public string? Progress { get; set; }
    public string? DCCommPkgStatus { get; set; }
    
    public string EventType => PcsEventConstants.CommPkgCreateOrUpdate;
}
