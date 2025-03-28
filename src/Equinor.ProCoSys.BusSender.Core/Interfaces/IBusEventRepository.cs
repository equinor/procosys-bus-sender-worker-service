using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IBusEventRepository
{
    Task<List<BusEvent>> GetEarliestUnProcessedEventChunk();
    Task<long> GetUnProcessedCount();
    Task<DateTime> GetOldestEvent();
    void SetPlants(List<string> plants);
}
