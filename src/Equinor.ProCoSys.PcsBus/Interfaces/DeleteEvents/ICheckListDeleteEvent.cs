using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces.DeleteEvents;

public interface IChecklistDeleteEventV1 : IHasEventType
{
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
}
