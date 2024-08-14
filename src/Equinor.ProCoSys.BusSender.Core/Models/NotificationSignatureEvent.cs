using Equinor.ProCoSys.PcsServiceBus;
using System;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class NotificationSignatureEvent : INotificationSignatureEventV1
{
    public string EventType => PcsEventConstants.NotificationSignatureCreateOrUpdate;
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public Guid ProjectGuid { get; init; }
    public Guid NotificationGuid { get; init; }
    public string? SignatureRoleCode { get; init; }
    public long Sequence { get; init; }
    public string? Status { get; init; }
    public Guid? SignerPersonOid { get; init; }
    public string? SignerFunctionalRoleCode { get; init; }
    public Guid? SignedByOid { get; init; }
    public DateTime? SignedAt { get; init; }
    public DateTime LastUpdated { get; init; }
}
