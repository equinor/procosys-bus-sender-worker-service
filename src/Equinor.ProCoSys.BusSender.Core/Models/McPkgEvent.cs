using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class McPkgEvent : IMcPkgEventV1
{
    public string EventType => PcsEventConstants.McPkgCreateOrUpdate;
    public string? AreaCode { get; init; }
    public Guid CommPkgGuid { get; init; }
    public string CommPkgNo { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? Description { get; init; }
    public string DisciplineCode { get; init; }
    public bool IsVoided { get; init; }
    public DateTime LastUpdated { get; init; }
    public long McPkgId { get; init; }
    public string McPkgNo { get; init; }
    public string McStatus { get; init; }
    public string? Phase { get; init; }
    public string Plant { get; init; }
    public string PlantName { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public string? Remark { get; init; }
    public string ResponsibleCode { get; init; }
}
