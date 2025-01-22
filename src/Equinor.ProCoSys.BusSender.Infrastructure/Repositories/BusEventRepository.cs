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

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class BusEventRepository : IBusEventRepository
{
    private const string InstanceNameConfigKey = "InstanceName";
    private const string MessageChunkSizeConfigKey = "MessageChunkSize";
    private readonly DbSet<BusEvent> _busEvents;
    private readonly int _messageChunkSize;
    private readonly List<string> _plants;
    private readonly string? _instanceName;

    public BusEventRepository(BusSenderServiceContext context, IPlantService plantService)
    {
        var configuration = plantService.GetConfiguration();
        _messageChunkSize = int.Parse(configuration[MessageChunkSizeConfigKey] ?? "200");
        _busEvents = context.BusEvents;
        _instanceName = string.IsNullOrEmpty(configuration[InstanceNameConfigKey]) ? PcsServiceBusInstanceConstants.DefaultInstanceName : configuration[InstanceNameConfigKey];
        _plants = plantService.GetPlantsHandledByCurrentInstance();
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
