using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IMcPkgEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string PlantName { get; set; }
    string ProjectName { get; set; }
    string McPkgNo { get; set; }
    long McPkgId { get; set; }
    string CommPkgNo { get; set; }
    Guid CommPkgGuid { get; set; }
    string? Description { get; set; }
    string? Remark { get; set; }
    string ResponsibleCode { get; set; }
    string? ResponsibleDescription { get; set; }
    string? AreaCode { get; set; }
    string? AreaDescription { get; set; }
    string Discipline { get; set; }
    string McStatus { get; set; }
    string? Phase { get; set; }
    bool IsVoided { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime LastUpdated { get; set; }
}
