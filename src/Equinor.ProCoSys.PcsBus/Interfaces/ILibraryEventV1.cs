using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface ILibraryEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    long LibraryId { get; init; }
    int? ParentId { get; init; }
    Guid? ParentGuid { get; init; }
    string Code { get; init; }
    string? Description { get; init; }
    bool IsVoided { get; init; }
    string Type { get; init; }
    DateTime LastUpdated { get; init; }
}
