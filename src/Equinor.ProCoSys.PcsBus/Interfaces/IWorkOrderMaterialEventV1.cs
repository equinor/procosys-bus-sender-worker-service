using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IWorkOrderMaterialEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    string WoNo { get; init; }
    long WoId { get; init; }
    Guid WoGuid { get; init; }
    string ItemNo { get; init; }
    string? TagNo { get; init; }
    long? TagId { get; init; }
    Guid? TagGuid { get; init; }
    string TagRegisterCode { get; init; }
    long? StockId { get; init; }
    Guid? StockGuid { get; init; }
    double? Quantity { get; init; }
    string? UnitName { get; init; }
    string? UnitDescription { get; init; }
    string? AdditionalInformation { get; init; }
    DateOnly? RequiredDate { get; init; }
    DateOnly? EstimatedAvailableDate { get; init; }
    bool? Available { get; init; }
    string? MaterialStatus { get; init; }
    string? StockLocation { get; init; }
    DateTime LastUpdated { get; init; }
}
