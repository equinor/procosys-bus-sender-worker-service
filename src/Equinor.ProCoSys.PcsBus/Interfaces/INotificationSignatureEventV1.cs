using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface INotificationSignatureEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    Guid ProjectGuid { get; init; }
    Guid NotificationGuid { get; init; }
    string? SignatureRoleCode { get; init; }
    long Sequence { get; init; }
    Guid? SignerPersonOid { get; init; }
    string? SignerFunctionalRoleCode { get; init; }
    Guid? SignedByOid { get; init; }
    DateTime? SignedAt { get; init; }
    DateTime LastUpdated { get; init; }
}
