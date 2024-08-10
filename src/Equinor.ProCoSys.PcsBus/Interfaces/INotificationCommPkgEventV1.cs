using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface INotificationCommPkgEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    Guid ProjectGuid { get; init; }
    Guid NotificationGuid { get; init; }
    Guid CommPkgGuid { get; init; }
    string RelationshipType { get; init; }
    public DateTime LastUpdated { get; init; }
}
