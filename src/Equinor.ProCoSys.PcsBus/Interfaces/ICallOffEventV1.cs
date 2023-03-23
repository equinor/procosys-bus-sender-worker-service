using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ICallOffEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    long CallOffId { get; set; }
    long PackageId { get; set; }
    string PurchaseOrderNo { get; set; }
    bool IsCompleted { get; set; }
    bool UseMcScope { get; set; }
    DateTime LastUpdated { get; set; }
    bool IsVoided { get; set; }
    DateTime CreatedAt { get; set; }
    string? CallOffNo { get; set; }
    string? Description { get; set; }
    string? ResponsibleCode { get; set; }
    string? ContractorCode { get; set; }
    string? SupplierCode { get; set; }
    int? EstimatedTagCount { get; set; }
    DateOnly? FATPlanned { get; set; }
    DateOnly? PackagePlannedDelivery { get; set; }
    DateOnly? PackageActualDelivery { get; set; }
    DateOnly? PackageClosed { get; set; }
    DateOnly? McDossierSent { get; set; }
    DateOnly? McDossierReceived { get; set; }
}
