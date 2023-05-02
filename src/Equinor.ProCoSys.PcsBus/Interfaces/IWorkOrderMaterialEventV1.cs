using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IWorkOrderMaterialEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string ProjectName { get; set; }
    string WoNo { get; set; }
    long WoId { get; set; }
    Guid WoGuid { get; set; }
    string ItemNo { get; set; }
    string? TagNo { get; set; }
    long? TagId { get; set; }
    Guid? TagGuid { get; set; }
    string TagRegisterCode { get; set; }
    long? StockId { get; set; }
    double? Quantity { get; set; }
    string? UnitName { get; set; }
    string? UnitDescription { get; set; }
    string? AdditionalInformation { get; set; }
    DateOnly? RequiredDate { get; set; }
    DateOnly? EstimatedAvailableDate { get; set; }
    bool? Available { get; set; }
    string? MaterialStatus { get; set; }
    string? StockLocation { get; set; }
    DateTime LastUpdated { get; set; }
}
