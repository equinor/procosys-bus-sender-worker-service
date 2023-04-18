using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IHeatTraceEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    long HeatTraceId { get; set; }
    long CableId { get; set; }
    Guid CableGuid { get; set; }
    string CableNo { get; set; }
    long TagId { get; set; }
    Guid TagGuid { get; set; }
    string TagNo { get; set; }
    string? SpoolNo { get; set; }
    DateTime LastUpdated { get; set; }
}
