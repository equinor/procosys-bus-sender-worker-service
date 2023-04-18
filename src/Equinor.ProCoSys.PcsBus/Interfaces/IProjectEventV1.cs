using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IProjectEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string ProjectName { get; set; }
    bool IsClosed { get; set; }
    string? Description { get; set; }
    DateTime LastUpdated { get; set; }
}
