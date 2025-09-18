using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ILibraryToLibraryEventV1 : IHasEventType
{
    Guid ProCoSysGuid { get; init; }
    string Plant { get; init; }
    string Role { get; init; }
    string Association { get; init; }
    Guid LibraryGuid { get; init; }
    Guid RelatedLibraryGuid { get; init; }
    DateTime LastUpdated { get; init; }
}
