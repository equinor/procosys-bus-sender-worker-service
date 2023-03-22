using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IMcPkgMilestoneEventV1 : IHasEventType
{
    string Plant { get; }
    Guid ProCoSysGuid { get; }
    string? PlantName { get; }
    string? ProjectName { get; }
    Guid McPkgGuid { get; }
    string? McPkgNo { get; }
    string Code { get; }
    DateTime? ActualDate { get; }
    DateOnly? PlannedDate { get; }
    DateOnly? ForecastDate { get; }
    string? Remark { get; }
    bool? IsSent { get; }
    bool? IsAccepted { get; }
    bool? IsRejected { get; }
    DateTime LastUpdated { get; }
}
