using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ICommPkgMilestoneEvent : IHasEventType
{ 
    string Plant { get; }
    Guid ProCoSysGuid { get; }
    string? PlantName { get; }
    string? ProjectName { get; }
    Guid CommPkgGuid { get; }
    string? CommPkgNo { get; }
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
