using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable 8618
public class PipingRevisionEvent : IPipingRevisionEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public long PipingRevisionId { get; set; }
    public int Revision { get; set; }
    public string McPkgNo { get; set; }
    public Guid McPkgNoGuid { get; set; }
    public string ProjectName { get; set; }
    public double? MaxDesignPressure { get; set; }
    public double? MaxTestPressure { get; set; }
    public string? Comments { get; set; }
    public string? TestISODocumentNo { get; set; }
    public Guid? TestISODocumentGuid { get; set; }
    public int? TestISORevision { get; set; }
    public string? PurchaseOrderNo { get; set; }
    public string? CallOffNo { get; set; }
    public Guid? CallOffGuid { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.PipingRevisionCreateOrUpdate;
}
