using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class WorkOrderMilestoneEvent : IWorkOrderMilestoneEventV1
{
    public string EventType => PcsEventConstants.WorkOrderMilestoneCreateOrUpdate;
    public string Code { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateOnly? MilestoneDate { get; set; }
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public string? SignedByAzureOid { get; set; }
    public Guid? WoGuid { get; set; }
    public long WoId { get; set; }
    public string WoNo { get; set; }
}
