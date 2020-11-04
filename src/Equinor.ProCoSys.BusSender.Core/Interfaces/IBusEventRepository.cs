using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Core.Models;

namespace Equinor.ProCoSys.BusSender.Core.Interfaces
{
    public interface IBusEventRepository
    {
        Task<List<BusEvent>> GetEarliestUnProcessedEventChunk();
    }
}
