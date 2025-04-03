using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IBusEventRepository
{
    Task<List<BusEvent>> GetEarliestUnProcessedEventChunk(bool ignoreFilter = false);
    Task<long> GetUnProcessedCount(bool ignoreFilter = false);
    Task<DateTime> GetOldestEvent(bool ignoreFilter = false);
    void SetPlants(List<string> plants);
}
