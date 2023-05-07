using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface ICallOffEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    long CallOffId { get; init; }
    long PackageId { get; init; }
    string PurchaseOrderNo { get; init; }
    bool IsCompleted { get; init; }
    bool UseMcScope { get; init; }
    DateTime LastUpdated { get; init; }
    bool IsVoided { get; init; }
    DateTime CreatedAt { get; init; }
    string? CallOffNo { get; init; }
    string? Description { get; init; }
    string? ResponsibleCode { get; init; }
    string? ContractorCode { get; init; }
    string? SupplierCode { get; init; }
    int? EstimatedTagCount { get; init; }
    DateOnly? FATPlanned { get; init; }
    DateOnly? PackagePlannedDelivery { get; init; }
    DateOnly? PackageActualDelivery { get; init; }
    DateOnly? PackageClosed { get; init; }
    DateOnly? McDossierSent { get; init; }
    DateOnly? McDossierReceived { get; init; }
}
