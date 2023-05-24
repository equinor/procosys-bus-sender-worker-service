using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IMcPkgMilestoneEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string? PlantName { get; init; }
    string? ProjectName { get; init; }
    Guid McPkgGuid { get; init; }
    string? McPkgNo { get; init; }
    string Code { get; init; }
    DateTime? ActualDate { get; init; }
    DateOnly? PlannedDate { get; init; }
    DateOnly? ForecastDate { get; init; }
    string? Remark { get; init; }
    bool? IsSent { get; init; }
    bool? IsAccepted { get; init; }
    bool? IsRejected { get; init; }
    DateTime LastUpdated { get; init; }
}
