using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface INotificationEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    long NotificationId { get; init; }
    string NotificationNo { get; init; }
    string? NotificationType { get; init; }
    public string? DocumentType { get; init; }

    string? Title { get; init; }
    public string? ResponsibleContractor { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastUpdated { get; init; }
}
