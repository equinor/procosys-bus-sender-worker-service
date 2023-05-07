using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class StockEvent : IStockEventV1
{
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public long StockId { get; init; }
    public string StockNo { get; init; }
    public string Description { get; init; }
    public DateTime LastUpdated { get; init; }
    
    public string EventType => PcsEventConstants.StockCreateOrUpdate;
}
