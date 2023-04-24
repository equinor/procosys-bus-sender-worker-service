using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IStockEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    long StockId { get; set; }  
    string StockNo { get; set; }
    string Description { get; set; }
    DateTime LastUpdated { get; set; }
}
