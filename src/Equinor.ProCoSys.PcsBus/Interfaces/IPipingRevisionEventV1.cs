using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IPipingRevisionEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    long PipingRevisionId { get; set; }
    int Revision { get; set; }
    string McPkgNo { get; set; }
    Guid McPkgNoGuid { get; set; }
    string ProjectName { get; set; }
    double? MaxDesignPressure { get; set; }
    double? MaxTestPressure { get; set; }
    string? Comments { get; set; }
    string? TestISODocumentNo { get; set; }
    Guid? TestISODocumentGuid { get; set; }
    int? TestISORevision { get; set; }
    string? PurchaseOrderNo { get; set; }
    string? CallOffNo { get; set; }
    Guid? CallOffGuid { get; set; }
    DateTime LastUpdated { get; set; }
}

