using Equinor.ProCoSys.PcsServiceBus;
using System;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class NotificationCommPkgEvent : INotificationCommPkgEventV1
{
    public string EventType => PcsEventConstants.NotificationCommPkgCreateOrUpdate;
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public Guid ProjectGuid { get; init; }
    public Guid NotificationGuid { get; init; }
    public Guid CommPkgGuid { get; init; }
    public DateTime LastUpdated { get; init; }
}
