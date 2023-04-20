using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable 8618
public class ProjectEvent : IProjectEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public bool IsClosed { get; set; }
    public string? Description { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.ProjectCreateOrUpdate;
}
