using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable 8618
public class PipingRevisionEvent : IPipingRevisionEventV1
{
    public string EventType => PcsEventConstants.PipingRevisionCreateOrUpdate;
    public Guid? CallOffGuid { get; init; }
    public string? CallOffNo { get; init; }
    public string? Comments { get; init; }
    public DateTime LastUpdated { get; init; }
    public double? MaxDesignPressure { get; init; }
    public double? MaxTestPressure { get; init; }
    public string McPkgNo { get; init; }
    public Guid McPkgNoGuid { get; init; }
    public long PipingRevisionId { get; init; }
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public string? PurchaseOrderNo { get; init; }
    public int Revision { get; init; }
    public Guid? TestISODocumentGuid { get; init; }
    public string? TestISODocumentNo { get; init; }
    public int? TestISORevision { get; init; }
}
