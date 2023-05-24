using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class WorkOrderMilestoneEvent : IWorkOrderMilestoneEventV1
{
    public string EventType => PcsEventConstants.WorkOrderMilestoneCreateOrUpdate;
    public string Code { get; init; }
    public DateTime LastUpdated { get; init; }
    public DateOnly? MilestoneDate { get; init; }
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public string? SignedByAzureOid { get; init; }
    public Guid? WoGuid { get; init; }
    public long WoId { get; init; }
    public string WoNo { get; init; }
}
