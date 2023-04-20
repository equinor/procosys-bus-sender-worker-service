using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface ILibraryEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    long LibraryId { get; set; }
    int? ParentId { get; set; }
    Guid? ParentGuid { get; set; }
    string Code { get; set; }
    string? Description { get; set; }
    bool IsVoided { get; set; }
    string Type { get; set; }
    DateTime LastUpdated { get; set; }
}
