using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IIpoCommPkgV1 : IHasEventType
{
    Guid ProCoSysGuid { get; init; }
    string Plant { get; init; }
    string ProjectName { get; init; }
    Guid InvitationGuid { get; init; }
    DateTime CreatedAtUtc { get; init; }
    Guid CommPkgGuid { get; init; }
}
