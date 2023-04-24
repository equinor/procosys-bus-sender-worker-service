using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class WorkOrderMaterialEvent : IWorkOrderMaterialEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public string WoNo { get; set; }
    public long WoId { get; set; }
    public Guid WoGuid { get; set; }
    public string ItemNo { get; set; }
    public string? TagNo { get; set; }
    public long? TagId { get; set; }
    public Guid? TagGuid { get; set; }
    public string TagRegisterCode { get; set; }
    public long? StockId { get; set; }
    public double? Quantity { get; set; }
    public string? UnitName { get; set; }
    public string? UnitDescription { get; set; }
    public string? AdditionalInformation { get; set; }
    public DateOnly? RequiredDate { get; set; }
    public DateOnly? EstimatedAvailableDate { get; set; }
    public bool? Available { get; set; }
    public string? MaterialStatus { get; set; }
    public string? StockLocation { get; set; }
    public DateTime LastUpdated { get; set; }
    public string EventType => PcsEventConstants.WorkOrderMaterialCreateOrUpdate;
}
