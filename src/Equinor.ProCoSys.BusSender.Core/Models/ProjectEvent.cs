using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable 8618
public class ProjectEvent : IProjectEventV1
{
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public bool IsClosed { get; init; }
    public string? Description { get; init; }
    public DateTime LastUpdated { get; init; }
    
    public string EventType => PcsEventConstants.ProjectCreateOrUpdate;
}
