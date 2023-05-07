using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface ICommPkgTaskEventV1 : IHasEventType
{
    string Plant { get; init; }
    string ProjectName { get; init; }
    Guid ProCoSysGuid { get; init; }
    Guid TaskGuid { get; init; }
    Guid CommPkgGuid { get; init; }
    string CommPkgNo { get; init; }
    DateTime LastUpdated { get; init; }
}
