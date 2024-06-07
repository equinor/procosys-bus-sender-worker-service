using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface ICommPkgEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string PlantName { get; init; }
    Guid ProjectGuid { get; init; }
    string ProjectName { get; init; }
    string CommPkgNo { get; init; }
    long CommPkgId { get; init; }
    string Description { get; init; }
    string CommPkgStatus { get; init; }
    bool IsVoided { get; init; }
    DateTime LastUpdated { get; init; }
    DateTime CreatedAt { get; init; }
    string? DescriptionOfWork { get; init; }
    string? Remark { get; init; }
    string ResponsibleCode { get; init; }
    string? ResponsibleDescription { get; init; }
    string? AreaCode { get; init; }
    string? AreaDescription { get; init; }
    string? Phase { get; init; }
    string? CommissioningIdentifier { get; init; }
    bool? Demolition { get; init; }
    string? Priority1 { get; init; }
    string? Priority2 { get; init; }
    string? Priority3 { get; init; }
    string? Progress { get; init; }
    string? DCCommPkgStatus { get; init; }
}
