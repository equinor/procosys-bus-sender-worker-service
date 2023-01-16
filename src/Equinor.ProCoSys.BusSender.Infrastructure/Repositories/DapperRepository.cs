using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
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

    public async Task<IEnumerable<T>> Query<T>(string queryString, string objectId) where T : IHasEventType
    {
        var connection = _context.Database.GetDbConnection();
        if (_context.Database.GetDbConnection().State != ConnectionState.Open)
        {
            await _context.Database.OpenConnectionAsync();
        }

        var checklists = connection.Query<T>(queryString).ToList();
        if (checklists.Count == 0)
        {
            _logger.LogError("Object/Entity with id {objectId} did not return anything", objectId);
            return null;
        }
        return checklists;
    }


}
