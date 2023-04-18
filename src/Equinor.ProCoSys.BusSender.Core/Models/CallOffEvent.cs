using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class CallOffEvent : ICallOffEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public long CallOffId { get; set; }
    public long PackageId { get; set; }
    public string PurchaseOrderNo { get; set; }
    public bool IsCompleted { get; set; }
    public bool UseMcScope { get; set; }
    public DateTime LastUpdated { get; set; }
    public bool IsVoided { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CallOffNo { get; set; }
    public string? Description { get; set; }
    public string? ResponsibleCode { get; set; }
    public string? ContractorCode { get; set; }
    public string? SupplierCode { get; set; }
    public int? EstimatedTagCount { get; set; }
    public DateOnly? FATPlanned { get; set; }
    public DateOnly? PackagePlannedDelivery { get; set; }
    public DateOnly? PackageActualDelivery { get; set; }
    public DateOnly? PackageClosed { get; set; }
    public DateOnly? McDossierSent { get; set; }
    public DateOnly? McDossierReceived { get; set; }
    public string EventType => PcsEventConstants.CallOffCreateOrUpdate;
}
