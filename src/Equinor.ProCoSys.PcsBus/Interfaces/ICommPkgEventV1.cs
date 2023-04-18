using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface ICommPkgEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string PlantName { get; set; }
    string ProjectName { get; set; }
    string CommPkgNo { get; set; }
    long CommPkgId { get; set; }
    string Description { get; set; }
    string CommPkgStatus { get; set; }
    bool IsVoided { get; set; }
    DateTime LastUpdated { get; set; }
    DateTime CreatedAt { get; set; }
    string? DescriptionOfWork { get; set; }
    string? Remark { get; set; }
    string ResponsibleCode { get; set; }
    string? ResponsibleDescription { get; set; }
    string? AreaCode { get; set; }
    string? AreaDescription { get; set; }
    string? Phase { get; set; }
    string? CommissioningIdentifier { get; set; }
    bool? Demolition { get; set; }
    string? Priority1 { get; set; }
    string? Priority2 { get; set; }
    string? Priority3 { get; set; }
    string? Progress { get; set; }
    string? DCCommPkgStatus { get; set; }
}
