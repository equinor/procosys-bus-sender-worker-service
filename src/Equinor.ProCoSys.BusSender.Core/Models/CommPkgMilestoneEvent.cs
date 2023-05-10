using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class CommPkgMilestoneEvent : ICommPkgMilestoneEventV1
{
    public DateTime? ActualDate { get; init; }
    public string Code { get; init; }
    public Guid CommPkgGuid { get; init; }
    public string? CommPkgNo { get; init; }
    public DateOnly? ForecastDate { get; init; }
    public bool? IsAccepted { get; init; }
    public bool? IsRejected { get; init; }
    public bool? IsSent { get; init; }
    public DateTime LastUpdated { get; init; }
    public DateOnly? PlannedDate { get; init; }
    public string Plant { get; init; }
    public string? PlantName { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string? ProjectName { get; init; }
    public string? Remark { get; init; }

    public string EventType => PcsEventConstants.MilestoneCreateOrUpdate;
}
