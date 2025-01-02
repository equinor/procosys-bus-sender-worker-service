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

    public async Task<List<BusEvent>> GetEarliestUnProcessedEventChunk()
    {
        return await GetUnProcessedFilteredQueryable()
            .OrderBy(e => e.Created)
            .Take(_messageChunkSize)
            .ToListAsync();
    }

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
        IQueryable<BusEvent> query;

        if (_plants.Contains(PcsServiceBusInstanceConstants.Plant) && _plants.Contains(PcsServiceBusInstanceConstants.NoPlant))
        {
            query = _busEvents.Where(e => e.Status == Status.UnProcessed);
        }
        else if (_plants.Contains(PcsServiceBusInstanceConstants.NoPlant))
        {
            query = _busEvents.Where(e => e.Status == Status.UnProcessed && (e.Plant == null || _plants.Contains(e.Plant)));
        }
        else if (_plants.Contains(PcsServiceBusInstanceConstants.Plant))
        {
            query = _busEvents.Where(e => e.Status == Status.UnProcessed && e.Plant != null);
        }
        else
        {
            query = _busEvents.Where(e => e.Status == Status.UnProcessed && e.Plant != null && _plants.Contains(e.Plant));
        }
        return query;
    }

    private List<string> GetPlantsFromConfiguration(IConfiguration configuration)
    {
        var plantsString = configuration["MessageSites"];
        if (!string.IsNullOrWhiteSpace(plantsString))
        {
            return plantsString.Split(',').ToList();
        }
        else
        {
            return new List<string> { PcsServiceBusInstanceConstants.Plant, PcsServiceBusInstanceConstants.NoPlant };
        }
    }

    public string? GetInstanceName() => _instanceName;
    public string GetPlants() => string.Join(",", _plants);
}
