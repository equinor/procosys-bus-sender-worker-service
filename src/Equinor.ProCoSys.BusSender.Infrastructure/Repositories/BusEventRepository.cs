using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class BusEventRepository : IBusEventRepository
{
    private readonly DbSet<BusEvent> _busEvents;
    private readonly int _messageChunkSize;

    public BusEventRepository(BusSenderServiceContext context, IConfiguration configuration)
    {
        _messageChunkSize = int.Parse(configuration["MessageChunkSize"]?? "200");
        _busEvents = context.BusEvents;
    }

    public async Task<List<BusEvent>> GetEarliestUnProcessedEventChunk()
    {
        var events = await _busEvents.Where(e => e.Status == Status.UnProcessed)
            .OrderBy(e => e.Created)
            .Take(_messageChunkSize)
            .ToListAsync();
        return events;
    }

    public async Task<long> GetUnProcessedCount() => 
        await _busEvents.Where(e => e.Status == Status.UnProcessed)
            .CountAsync();

    public async Task<DateTime> GetOldestEvent() =>
        await _busEvents.Where(e => e.Status == Status.UnProcessed)
            .OrderBy(e => e.Created)
            .Select(e => e.Created)
            .FirstOrDefaultAsync();
}
