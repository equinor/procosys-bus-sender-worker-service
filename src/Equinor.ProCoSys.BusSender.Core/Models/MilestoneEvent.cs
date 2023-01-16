using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class MilestoneEvent : IMilestoneEvent
{
    public string Plant { get;  }
    public Guid ProCoSysGuid { get;  }
    public string? PlantName { get; }
    public string? ProjectName { get; }
    public Guid? CommPkgGuid { get; }
    public Guid? McPkgGuid { get; }
    public string? CommPkgNo { get; }
    public string? McPkgNo { get; }
    public string Code { get; }
    public DateOnly? ActualDate { get; }
    public DateOnly? PlannedDate { get; }
    public DateOnly? ForecastDate { get; }
    public string? Remark { get; }
    public bool? IsSent { get; }
    public bool? IsAccepted { get; }
    public bool? IsRejected { get; }
    public DateTime LastUpdated { get; }

    public string EventType => PcsEventConstants.MilestoneCreateOrUpdate;
}
