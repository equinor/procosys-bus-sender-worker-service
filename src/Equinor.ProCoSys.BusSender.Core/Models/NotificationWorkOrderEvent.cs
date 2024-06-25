using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class NotificationWorkOrderEvent : INotificationWorkOrderEventV1
{
    public string EventType => PcsEventConstants.NotificationWorkOrderCreateOrUpdate;
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public Guid NotificationGuid { get; set; }
    public Guid WorkOrderGuid { get; set; }
}
