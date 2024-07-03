using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IPunchPriorityLibRelationEventV1 : IHasEventType
{
    string Plant { get; }
    Guid ProCoSysGuid { get; }
    Guid CommPriorityGuid { get;}
    DateTime LastUpdated { get; }
}
