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

    public async Task<string> GetCallOffMessage(long callOffId) => 
        await ExecuteQuery(CallOffQuery.GetQuery(callOffId), callOffId.ToString());

    public async Task<string> GetCheckListMessage(long checkListId) => 
        await ExecuteQuery(ChecklistQuery.GetQuery(checkListId), checkListId.ToString());

    public async Task<string> GetCommPkgQueryMessage(long commPkgId, long documentId) =>
        await ExecuteQuery(QueryCommPkgQuery.GetQuery(commPkgId,documentId), commPkgId+","+documentId);

    public async Task<string> GetCommPkgOperationMessage(long commPkgId) =>
        await ExecuteQuery(CommPkgOperationQuery.GetQuery(commPkgId), commPkgId.ToString());

    public async Task<string> GetDocumentMessage(long documentId) =>
        await ExecuteQuery(DocumentQuery.GetQuery(documentId), documentId.ToString());

    public async Task<string> GetMilestoneMessage(long elementId,long milestoneId) =>
        await ExecuteQuery(MilestonesQuery.GetQuery(elementId, milestoneId),elementId+","+milestoneId);

    public async Task<string> GetLoopContentMessage(long loopContentId) => 
        await ExecuteQuery(LoopContentQuery.GetQuery(loopContentId),loopContentId.ToString());

    public async Task<string> GetPipingRevisionMessage(long pipeRevisionId) =>
        await ExecuteQuery(PipingRevisionQuery.GetQuery(pipeRevisionId), pipeRevisionId.ToString());

    public async Task<string> GetPipeTestMessage(long pipeRevisionId,long pipeTestLibraryId) =>
        await ExecuteQuery(PipeTestQuery.GetQuery(pipeRevisionId,pipeTestLibraryId), pipeRevisionId+","+pipeTestLibraryId);

    public async Task<string> GetHeatTraceMessage(long id) =>
        await ExecuteQuery(HeatTraceQuery.GetQuery(id), id.ToString());

    public async Task<string> GetPipingSpoolMessage(long pipingSpoolId) =>
        await ExecuteQuery(PipingSpoolQuery.GetQuery(pipingSpoolId), pipingSpoolId.ToString());

    public async Task<string> GetQueryMessage(long documentId) =>
        await ExecuteQuery(QueryQuery.GetQuery(documentId), documentId.ToString());

    public async Task<string> GetQuerySignatureMessage(long querySignatureId) =>
        await ExecuteQuery(QuerySignatureQuery.GetQuery(querySignatureId), querySignatureId.ToString());

    public async Task<string> GetStockMessage(long stockId) =>
        await ExecuteQuery(StockQuery.GetQuery(stockId), stockId.ToString());

    public async Task<string> GetSwcrMessage(long swcrId) =>
        await ExecuteQuery(SwcrQuery.GetQuery(swcrId), swcrId.ToString());

    public async Task<string> GetSwcrSignatureMessage(long swcrSignatureId) =>
        await ExecuteQuery(SwcrSignatureQuery.GetQuery(swcrSignatureId), swcrSignatureId.ToString());

    public async Task<string> GetWorkOrderChecklistMessage(long tagCheckId, long woId) =>
        await ExecuteQuery(WorkOrderChecklistsQuery.GetQuery(tagCheckId, woId), $"{tagCheckId},{woId}");

    public async Task<string> GetWorkOrderMaterialMessage(long woId) =>
        await ExecuteQuery(WorkOrderMaterialQuery.GetQuery(woId), woId.ToString());

    public async Task<string> GetWorkOrderMessage(long workOrderId) =>
        await ExecuteQuery(WorkOrderQuery.GetQuery(workOrderId), workOrderId.ToString());

    public async Task<string> GetWorkOrderCutOffMessage(long workOrderId, string cutoffWeek) =>
        await ExecuteQuery(WorkOrderCutoffQuery.GetQuery(workOrderId, cutoffWeek ), $"{workOrderId},{cutoffWeek}");

    public async Task<string> GetWorkOrderMilestoneMessage(long woId, long milestoneId) =>
        await ExecuteQuery(WorkOrderMilestoneQuery.GetQuery(woId, milestoneId), $"{woId},{milestoneId}");

    private async Task<string> ExecuteQuery(string queryString, string objectId)
    {
        await using var command = _context.Database.GetDbConnection().CreateCommand();
        command.CommandText = queryString;
        await _context.Database.OpenConnectionAsync();
        await using var result = await command.ExecuteReaderAsync();

        return await ExtractMessageFromResult(objectId, result);
    }
    private async Task<string> ExtractMessageFromResult(string objectId, DbDataReader result)
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
