using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class WorkOrderChecklistEvent : IWorkOrderChecklistEventV1
{
    public string EventType => PcsEventConstants.WorkOrderChecklistCreateOrUpdate;
    public Guid ChecklistGuid { get; init; }
    public long ChecklistId { get; init; }
    public DateTime LastUpdated { get; init; }
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public Guid WoGuid { get; init; }
    public long WoId { get; init; }
    public string WoNo { get; init; }
}
