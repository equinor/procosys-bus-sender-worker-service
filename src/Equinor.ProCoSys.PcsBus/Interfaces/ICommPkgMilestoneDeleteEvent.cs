using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ICommPkgMilestoneDeleteEvent : IHasEventType
{
    string Plant { get; }
    Guid ProCoSysGuid { get; }

}
