using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces.DeleteEvents;

public interface BaseDeleteEventV1 : IHasEventType
{
    string Plant { get; }
    Guid ProCoSysGuid { get; }

}
