using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    DateOnly? ActualDate { get; }
    DateOnly? PlannedDate { get; }
    DateOnly? ForecastDate { get; }
    string? Remark { get; }
    bool? IsSent { get; }
    bool? IsAccepted { get; }
    bool? IsRejected { get; }
    DateTime LastUpdated { get; }
}
