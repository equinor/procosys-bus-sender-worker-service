using System;
using System.Data.Common;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Equinor.ProCoSys.PcsServiceBus.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class BusSenderMessageRepository : IBusSenderMessageRepository
{
    private readonly BusSenderServiceContext _context;
    private readonly ILogger<BusSenderMessageRepository> _logger;
    public BusSenderMessageRepository(BusSenderServiceContext context, ILogger<BusSenderMessageRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<string> GetWorkOrderMessage(long workOrderId)
        => await ExecuteQuery(WorkOrderQuery.GetQuery(workOrderId), workOrderId);

    public async Task<string> GetQueryMessage(long documentId)
        => await ExecuteQuery(QueryQuery.GetQuery(documentId), documentId);

    public async Task<string> GetDocumentMessage(long documentId)
        => await ExecuteQuery(DocumentQuery.GetQuery(documentId), documentId);

    public async Task<string> GetCheckListMessage(long checkListId)
        => await ExecuteQuery(ChecklistQuery.GetQuery(checkListId), checkListId);

    private async Task<string> ExecuteQuery(string queryString, long objectId)
    {
        await using var command = _context.Database.GetDbConnection().CreateCommand();
        command.CommandText = queryString;
        await _context.Database.OpenConnectionAsync();
        await using var result = await command.ExecuteReaderAsync();

        return await ExtractMessageFromResult(objectId, result);
    }

    private async Task<string> ExtractMessageFromResult(long objectId, DbDataReader result)
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
