using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class WorkOrderMilestoneEvent : IWorkOrderMilestoneEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set;  }
    public long WoId { get; set; }
    public Guid? WoGuid { get; set; }
    public string WoNo { get; set; }
    public string Code { get; set; }
    public DateOnly? MilestoneDate { get; set; }
    public string? SignedByAzureOid { get; set; }
    public DateTime LastUpdated { get; set; }

    public string EventType => PcsEventConstants.WorkOrderMilestoneCreateOrUpdate;
}
