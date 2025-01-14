using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Equinor.ProCoSys.PcsServiceBus;
using Microsoft.Azure.Amqp.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class BusEventRepository : IBusEventRepository
{
    private readonly DbSet<BusEvent> _busEvents;
    private readonly int _messageChunkSize;
    private readonly List<string> _plants;
    private readonly string? _instanceName;

    public BusEventRepository(BusSenderServiceContext context, IPlantService plantService)
    {
        var configuration = plantService.GetConfiguration();
        _messageChunkSize = int.Parse(configuration["MessageChunkSize"]?? "200");
        _busEvents = context.BusEvents;
        _instanceName = string.IsNullOrEmpty(configuration["InstanceName"]) ? PcsServiceBusInstanceConstants.DefaultInstanceName : configuration["InstanceName"];
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

        if (containsPlant && containsNoPlant)
        {
            return query;
        }

        if (containsNoPlant)
        {
            return query.Where(e => e.Plant == null || _plants.Contains(e.Plant));
        }

        if (containsPlant)
        {
            return query.Where(e => e.Plant != null);
        }

        var test = query.Where(e => e.Plant != null).ToList();
        return query.Where(e => e.Plant != null && _plants.Contains(e.Plant));
    }
}
