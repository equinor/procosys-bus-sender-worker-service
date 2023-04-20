using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class TaskEvent : ITaskEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public Guid? TaskParentProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public int DocumentId { get; set; }
    public string Title { get; set; }
    public string TaskId { get; set; }
    public Guid ElementContentGuid { get; set; }
    public string Description { get; set; }
    public string Comments { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime? SignedAt { get; set; }
    public Guid? SignedBy { get; set; }
    
    public string EventType => PcsEventConstants.TaskCreateOrUpdate;
}
