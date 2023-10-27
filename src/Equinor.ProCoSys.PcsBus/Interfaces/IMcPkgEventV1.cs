using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IMcPkgEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string PlantName { get; init; }
    string ProjectName { get; init; }
    string McPkgNo { get; init; }
    long McPkgId { get; init; }
    string CommPkgNo { get; init; }
    Guid CommPkgGuid { get; init; }
    string? Description { get; init; }
    string? Remark { get; init; }
    string ResponsibleCode { get; init; }
    string? ResponsibleDescription { get; init; }
    string? AreaCode { get; init; }
    string? AreaDescription { get; init; }
    string Discipline { get; init; }
    string DisciplineCode { get; init; }
    string McStatus { get; init; }
    string? Phase { get; init; }
    bool IsVoided { get; init; }
    DateTime CreatedAt { get; init; }
    DateTime LastUpdated { get; init; }
}
