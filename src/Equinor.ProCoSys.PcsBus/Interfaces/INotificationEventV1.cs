using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface INotificationEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    public string NotificationId { get; set; }
    string DocumentNo { get; set; }
    string? NotificationType { get; set; }
    string? Title { get; set; }
}
