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
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class BusEventRepository : IBusEventRepository
{
    private readonly DbSet<BusEvent> _busEvents;
    private readonly int _messageChunkSize;
    private readonly List<string> _plants;
    private const string Plant = "PLANT";
    private const string NoPlant = "NOPLANT";


    public BusEventRepository(BusSenderServiceContext context, IConfiguration configuration)
    {
        _messageChunkSize = int.Parse(configuration["MessageChunkSize"]?? "200");
        _busEvents = context.BusEvents;
        _plants = GetPlantsFromConfiguration(configuration);
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
        if (_plants.Contains(Plant) && _plants.Contains(NoPlant))
        {
            return _busEvents.Where(e => e.Status == Status.UnProcessed);
        }
        else if (_plants.Contains(Plant))
        {
            return _busEvents.Where(e => e.Status == Status.UnProcessed && e.Plant != null);
        }
        else if (_plants.Contains(NoPlant))
        {
            return _busEvents.Where(e => e.Status == Status.UnProcessed && e.Plant == null);
        }
        else
        {
            return _busEvents.Where(e => e.Status == Status.UnProcessed && e.Plant != null && _plants.Contains(e.Plant));
        }
    }

    private static List<string> GetPlantsFromConfiguration(IConfiguration configuration)
    {
        var plantsString = configuration["MessageSites"];
        if (!string.IsNullOrWhiteSpace(plantsString))
        {
            return plantsString.Split(',').ToList();
        }
        else
        {
            return new List<string> { Plant, NoPlant };
        }
    }

    // Validate plants during initialization?
}
