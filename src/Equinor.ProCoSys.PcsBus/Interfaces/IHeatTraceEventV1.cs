using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IHeatTraceEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    long HeatTraceId { get; init; }
    long CableId { get; init; }
    Guid CableGuid { get; init; }
    string CableNo { get; init; }
    long TagId { get; init; }
    Guid TagGuid { get; init; }
    string TagNo { get; init; }
    string? SpoolNo { get; init; }
    DateTime LastUpdated { get; init; }
}
