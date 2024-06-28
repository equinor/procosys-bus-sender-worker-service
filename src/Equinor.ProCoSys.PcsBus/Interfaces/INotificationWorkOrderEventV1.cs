using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface INotificationWorkOrderEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    Guid NotificationGuid { get; init; }
    Guid WorkOrderGuid { get; init; }
    public DateTime LastUpdated { get; init; }
}
