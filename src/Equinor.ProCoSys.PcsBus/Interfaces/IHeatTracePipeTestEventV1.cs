using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IHeatTracePipeTestEventV1 : IHasEventType
{
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public Guid TagGuid { get; init; }
    public string Name { get; init; }
    public DateTime LastUpdated { get; init; }
}
