using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

internal class McPkgMilestoneEvent : IMcPkgMilestoneEventV1
{
    public string Plant { get;  }
    public Guid ProCoSysGuid { get;  }
    public string? PlantName { get; }
    public string? ProjectName { get; }
    public Guid McPkgGuid { get; }
    public string? McPkgNo { get; }
    public string Code { get; }
    public DateTime? ActualDate { get; }
    public DateOnly? PlannedDate { get; }
    public DateOnly? ForecastDate { get; }
    public string? Remark { get; }
    public bool? IsSent { get; }
    public bool? IsAccepted { get; }
    public bool? IsRejected { get; }
    public DateTime LastUpdated { get; }

    public string EventType => PcsEventConstants.MilestoneCreateOrUpdate;
}
