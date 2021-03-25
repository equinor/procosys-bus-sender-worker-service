using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces
{
    public interface IBusEventRepository
    {
        Task<List<BusEvent>> GetEarliestUnProcessedEventChunk();
    }
}
