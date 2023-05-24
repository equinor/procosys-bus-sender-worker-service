using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IPipingRevisionEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    long PipingRevisionId { get; init; }
    int Revision { get; init; }
    string McPkgNo { get; init; }
    Guid McPkgNoGuid { get; init; }
    string ProjectName { get; init; }
    double? MaxDesignPressure { get; init; }
    double? MaxTestPressure { get; init; }
    string? Comments { get; init; }
    string? TestISODocumentNo { get; init; }
    Guid? TestISODocumentGuid { get; init; }
    int? TestISORevision { get; init; }
    string? PurchaseOrderNo { get; init; }
    string? CallOffNo { get; init; }
    Guid? CallOffGuid { get; init; }
    DateTime LastUpdated { get; init; }
}
