﻿using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

internal class EventRepository : IEventRepository
{
    private readonly BusSenderServiceContext _context;
    private readonly ILogger<EventRepository> _logger;

    public EventRepository(BusSenderServiceContext context, ILogger<EventRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<T?> QuerySingle<T>(string queryString, string objectId) where T : IHasEventType
    {
        var connection = _context.Database.GetDbConnection();
        var connectionWasClosed = connection.State != ConnectionState.Open;
        if (connectionWasClosed)
        {
            await _context.Database.OpenConnectionAsync();
        }

        try
        {
            var events = connection.Query<T>(queryString).ToList();
            if (events.Count == 0)
            {
                _logger.LogError("Object/Entity with id {ObjectId} did not return anything", objectId);
                return default;
            }

            return events.Single();
        }
        finally
        {
            //If we open it, we have to close it.
            if (connectionWasClosed)
            {
                await _context.Database.CloseConnectionAsync();
            }
        }
    }
}
