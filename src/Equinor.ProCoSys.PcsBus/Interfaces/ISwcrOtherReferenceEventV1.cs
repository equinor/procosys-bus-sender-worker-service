using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ISwcrOtherReferenceEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    Guid LibraryGuid { get; init; }
    Guid SwcrGuid { get; init; }
    string Code { get; init; }
    string? Description { get; init; }
    DateTime LastUpdated { get; init; } 
}
