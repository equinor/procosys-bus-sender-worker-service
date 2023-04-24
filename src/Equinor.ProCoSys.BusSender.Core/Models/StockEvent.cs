using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class StockEvent : IStockEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public long StockId { get; set; }
    public string StockNo { get; set; }
    public string Description { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.StockCreateOrUpdate;
}
