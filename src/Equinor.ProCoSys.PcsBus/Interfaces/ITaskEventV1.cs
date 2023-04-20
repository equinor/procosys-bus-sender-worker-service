using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface ITaskEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    Guid? TaskParentProCoSysGuid { get; set; }
    string ProjectName { get; set; }
    int DocumentId { get; set; }
    string Title { get; set; }
    string TaskId { get; set; }
    Guid ElementContentGuid { get; set; }
    string Description { get; set; }
    string Comments { get; set; }
    DateTime LastUpdated { get; set; }
    DateTime? SignedAt { get; set; }
    Guid? SignedBy { get; set; }
}
