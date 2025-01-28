using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Equinor.ProCoSys.PcsServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class BusEventRepository : IBusEventRepository
{
    private readonly DbSet<BusEvent> _busEvents;
    private readonly int _messageChunkSize;
    private readonly List<string> _plants;

    public BusEventRepository(BusSenderServiceContext context, InstanceConfig instanceConfig, IOptions<InstanceOptions> instanceOptions)
    {
        _messageChunkSize = instanceOptions.Value.MessageChunkSize;
        _busEvents = context.BusEvents;
        _plants = instanceConfig.PlantsHandledByCurrentInstance;
    }

    public async Task<List<BusEvent>> GetEarliestUnProcessedEventChunk() => 
        await GetUnProcessedFilteredQueryable()
            .OrderBy(e => e.Created)
            .Take(_messageChunkSize)
            .ToListAsync();

    public async Task<long> GetUnProcessedCount() => 
        await GetUnProcessedFilteredQueryable()
            .CountAsync();

    public async Task<DateTime> GetOldestEvent() =>
        await GetUnProcessedFilteredQueryable()
            .OrderBy(e => e.Created)
            .Select(e => e.Created)
            .FirstOrDefaultAsync();

    private IQueryable<BusEvent> GetUnProcessedFilteredQueryable()
    {
        var query = _busEvents.Where(e => e.Status == Status.UnProcessed);
        var containsPlant = _plants.Contains(PcsServiceBusInstanceConstants.Plant);
        var containsNoPlant = _plants.Contains(PcsServiceBusInstanceConstants.NoPlant);

        // No filter. Query for unprocessed messages for all plants and messages without plant is returned.
        if (containsPlant && containsNoPlant)
        {
            return query;
        }

        // Filter on messages without any given plant or any specific plant provided for the instance.
        if (containsNoPlant)
        {
            return query.Where(e => e.Plant == null || _plants.Contains(e.Plant));
        }

        // Filter on messages for any plant, but not for messages without plant.
        if (containsPlant)
        {
            return query.Where(e => e.Plant != null);
        }

        // Filter on specific plant provided for the instance, but not for messages without plant.
        return query.Where(e => e.Plant != null && _plants.Contains(e.Plant));
    }
}
