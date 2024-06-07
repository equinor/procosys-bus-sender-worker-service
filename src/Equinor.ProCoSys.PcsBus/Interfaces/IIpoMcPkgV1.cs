using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IIpoMcPkgV1 : IHasEventType
{
    Guid ProCoSysGuid { get; init; }
    string Plant { get; init; }
    string ProjectName { get; init; }
    Guid InvitationGuid { get; init; }
    DateTime CreatedAtUtc { get; init; }
    public Guid McPkgGuid { get; init; }
}
