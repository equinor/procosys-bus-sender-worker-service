using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface INotificationSignatureEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    Guid ProjectGuid { get; init; }
    Guid NotificationGuid { get; init; }
    string SignatureRole { get; init; }
    long Sequence { get; init; }
    string Status { get; init; }
    Guid SignerPersonOid { get; init; }
    string SignerFunctionalRole { get; init; }
    Guid SignedByOid { get; init; }
    DateTime SignedAt { get; init; }
    DateTime LastUpdated { get; init; }
}
