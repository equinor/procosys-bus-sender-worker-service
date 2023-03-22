using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ICallOffEventV1
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    long CallOffId { get; set; }
    string CallOffNo { get; set; }
    long PackageId { get; set; }
    string PurchaseOrderNo { get; set; }
    bool IsCompleted { get; set; }
    bool UseMcScope { get; set; }
    string Description { get; set; }
    string ResponsibleCode { get; set; }
    string ContractorCode { get; set; }
    string SupplierCode { get; set; }
    int EstimatedTagCount { get; set; }
    DateTime FATPlanned { get; set; }
    DateTime PackagePlannedDelivery { get; set; }
    DateTime PackageActualDelivery { get; set; }
    DateTime PackageClosed { get; set; }
    DateTime McDossierSent { get; set; }
    DateTime McDossierReceived { get; set; }
    DateTime LastUpdated { get; set; }
    bool IsVoided { get; set; }
    DateTime CreatedAt { get; set; }
}
