using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class BusSenderRepository
{
    private readonly BusSenderServiceContext _context;
    private readonly ILogger<BusSenderRepository> _logger;

    public BusSenderRepository(BusSenderServiceContext context, ILogger<BusSenderRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> ExecuteQuery(string queryString, long objectId)
    {
        await using var command = _context.Database.GetDbConnection().CreateCommand();
        command.CommandText = queryString;
        await _context.Database.OpenConnectionAsync();
        await using var result = await command.ExecuteReaderAsync();

        return await ExtractQueryFromResult(objectId, result);
    }

    private async Task<string> ExtractQueryFromResult(long objectId, DbDataReader result)
    {
        //result.ReadAsync is expected to return true here, query for 1 objectId should return 1 and only 1 row. 
        if (!result.HasRows || !await result.ReadAsync() || result[0] is DBNull || result[0] is null)
        {
            _logger.LogError("Object/Entity with id {objectId} did not return anything", objectId);
            return null;
        }

        var queryResult = (string)result[0];

        //result.ReadAsync is expected to be false here, this is because there should be no more rows to read.
        if (await result.ReadAsync())
        {
            _logger.LogError("Object/Entity returned more than 1 row, this should not happen.");
        }

        return queryResult;
    }
}
