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
using Microsoft.Extensions.Configuration;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class BusEventRepository : IBusEventRepository
{
    private readonly DbSet<BusEvent> _busEvents;
    private readonly int _messageChunkSize;
    private List<string> _plants = new();

    public BusEventRepository(BusSenderServiceContext context, IConfiguration configuration)
    {
        _messageChunkSize = int.Parse(configuration["MessageChunkSize"] ?? "200");
        _busEvents = context.BusEvents;
    }

    public void SetPlants(List<string> plants) => _plants = plants;
    public List<string> GetPlants() => _plants;

    public async Task<List<BusEvent>> GetEarliestUnProcessedEventChunk(bool ignoreFilter = false) =>
        await GetUnProcessedFilteredQueryable(ignoreFilter)
            .OrderBy(e => e.Created)
            .Take(_messageChunkSize)
            .ToListAsync();

    public async Task<long> GetUnProcessedCount(bool ignoreFilter=false) => 
        await GetUnProcessedFilteredQueryable(ignoreFilter)
            .CountAsync();

    public async Task<DateTime> GetOldestEvent(bool ignoreFilter = false) =>
        await GetUnProcessedFilteredQueryable(ignoreFilter)
            .OrderBy(e => e.Created)
            .Select(e => e.Created)
            .FirstOrDefaultAsync();

    private IQueryable<BusEvent> GetUnProcessedFilteredQueryable(bool ignoreFilter = false)
    {
        var query = _busEvents.Where(e => e.Status == Status.UnProcessed);

        if (_plants == null || !_plants.Any() || ignoreFilter)
        {
            return query;
        }

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
