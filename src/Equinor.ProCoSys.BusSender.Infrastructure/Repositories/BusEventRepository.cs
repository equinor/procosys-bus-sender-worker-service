using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Core.Interfaces;
using Equinor.ProCoSys.BusSender.Core.Models;
using Equinor.ProCoSys.BusSender.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Equinor.ProCoSys.BusSender.Infrastructure.Repositories
{
    public class BusEventRepository : IBusEventRepository
    {
        private readonly DbSet<BusEvent> _busEvents;

        public BusEventRepository(BusSenderServiceContext context) => _busEvents = context.BusEvents;

        public async Task<List<BusEvent>> GetEarliestUnProcessedEventChunk()
        {
            var events = await _busEvents.Where(e => e.Sent == 0).OrderBy(e => e.Created).Take(30).ToListAsync();

            return events;
        }
    }
}
