using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class NotificationEvent : INotificationEventV1
{
    public string EventType => PcsEventConstants.NotificationCreateOrUpdate;
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public long NotificationId { get; init; }
    public string NotificationNo { get; init; }
    public string? NotificationType { get; init; }
    public string? DocumentType { get; init; }
    public string? Title { get; init; }
    public string? ResponsibleContractor { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime LastUpdated { get; init; }
}
