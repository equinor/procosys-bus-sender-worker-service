using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class WorkOrderMaterialEvent : IWorkOrderMaterialEventV1
{
    public string EventType => PcsEventConstants.WorkOrderMaterialCreateOrUpdate;
    public string? AdditionalInformation { get; init; }
    public bool? Available { get; init; }
    public DateOnly? EstimatedAvailableDate { get; init; }
    public string ItemNo { get; init; }
    public DateTime LastUpdated { get; init; }
    public string? MaterialStatus { get; init; }
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public double? Quantity { get; init; }
    public DateOnly? RequiredDate { get; init; }
    public long? StockId { get; init; }
    public string? StockLocation { get; init; }
    public Guid? TagGuid { get; init; }
    public long? TagId { get; init; }
    public string? TagNo { get; init; }
    public string TagRegisterCode { get; init; }
    public string? UnitDescription { get; init; }
    public string? UnitName { get; init; }
    public Guid WoGuid { get; init; }
    public long WoId { get; init; }
    public string WoNo { get; init; }
}
