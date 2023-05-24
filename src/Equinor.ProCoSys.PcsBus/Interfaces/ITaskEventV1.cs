using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface ITaskEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    Guid? TaskParentProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    int DocumentId { get; init; }
    string Title { get; init; }
    string TaskId { get; init; }
    Guid ElementContentGuid { get; init; }
    string Description { get; init; }
    string Comments { get; init; }
    DateTime LastUpdated { get; init; }
    DateTime? SignedAt { get; init; }
    Guid? SignedBy { get; init; }
}
