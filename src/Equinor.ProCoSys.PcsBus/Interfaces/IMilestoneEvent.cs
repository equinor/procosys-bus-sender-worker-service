using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IMilestoneEvent : IHasEventType
{
    string Plant { get; }
    Guid ProCoSysGuid { get; }
    string? PlantName { get; }
    string? ProjectName { get; }
    Guid? CommPkgGuid { get; }
    Guid? McPkgGuid { get; }
    string? CommPkgNo { get; }
    string? McPkgNo { get; }
    string Code { get; }
    DateOnly? ActualDate { get; }
    DateOnly? PlannedDate { get; }
    DateOnly? ForecastDate { get; }
    string? Remark { get; }
    bool? IsSent { get; }
    bool? IsAccepted { get; }
    bool? IsRejected { get; }
    DateTime LastUpdated { get; }
}
