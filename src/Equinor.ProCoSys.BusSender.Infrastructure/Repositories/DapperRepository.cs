using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

internal class DapperRepository : IDapperRepository
{
    private readonly BusSenderServiceContext _context;
    private readonly ILogger<DapperRepository> _logger;

    public DapperRepository(BusSenderServiceContext context, ILogger<DapperRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<T>> Query<T>(string queryString, string? objectId) where T : IHasEventType
    {
        var connection = _context.Database.GetDbConnection();
        if (_context.Database.GetDbConnection().State != ConnectionState.Open)
        {
            await _context.Database.OpenConnectionAsync();
        }

        var events = connection.Query<T>(queryString).ToList();
        if (events.Count == 0)
        {
            _logger.LogError("Object/Entity with id {objectId} did not return anything", objectId);
            return Enumerable.Empty<T>();
        }
        return events;
    }

    public async Task<T?> QuerySingle<T>(string queryString, string objectId) where T : IHasEventType
    {
        var connection = _context.Database.GetDbConnection();
        if (_context.Database.GetDbConnection().State != ConnectionState.Open)
        {
            await _context.Database.OpenConnectionAsync();
        }

        var events = connection.Query<T>(queryString).ToList();
        if (events.Count == 0)
        {
            _logger.LogError("Object/Entity with id {objectId} did not return anything", objectId);
            return default;
        }
        return events.Single();
    }
    
    public async Task<IEnumerable<TagEvent>> QueryTag(string queryString,string subQueryString)
    { 
        var connection = _context.Database.GetDbConnection();
        if (_context.Database.GetDbConnection().State != ConnectionState.Open)
        {
            await _context.Database.OpenConnectionAsync();
        }

        using (var multi = await connection.QueryMultipleAsync($"{queryString};{subQueryString}"))
        {
            var tags = multi.Read<TagEvent>().ToList();
            var tagDetails = multi.Read<dynamic>().ToList();
            var tagDetailsGrouped = tagDetails.GroupBy(td => td.TagId).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var tag in tags)
            {
                if (tagDetailsGrouped.TryGetValue(tag.TagId, out List<dynamic>? details))
                {
                    tag.TagDetails = details;
                }
            }

            if (tags.Count != 0)
            {
                return tags;
            }

            _logger.LogError("Query did not return anything");
            return Enumerable.Empty<TagEvent>();
        }
        
    }
}
