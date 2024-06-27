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
    public long NotificationId { get; set; }
    public string NotificationNo { get; set; }
    public string? NotificationType { get; set; }
    public string? Title { get; set; }
}
