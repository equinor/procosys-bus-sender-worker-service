using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

#pragma warning disable CS8618
[UsedImplicitly]
public record PunchPriorityLibRelationEvent : IHasEventType
{
    public string EventType => PcsEventConstants.PunchPriorityLibRelationCreateOrUpdate;
    
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public Guid CommPriorityGuid { get; init; }
    public DateTime LastUpdated { get; init; }
}
