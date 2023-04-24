using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class WorkOrderChecklistEvent : IWorkOrderChecklistEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public long ChecklistId { get; set; }
    public Guid ChecklistGuid { get; set; }
    public long WoId { get; set; }
    public Guid WoGuid { get; set; }
    public string WoNo { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.WorkOrderChecklistCreateOrUpdate;
}
