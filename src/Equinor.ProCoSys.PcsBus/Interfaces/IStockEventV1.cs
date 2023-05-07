using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IStockEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    long StockId { get; init; }  
    string StockNo { get; init; }
    string Description { get; init; }
    DateTime LastUpdated { get; init; }
}
