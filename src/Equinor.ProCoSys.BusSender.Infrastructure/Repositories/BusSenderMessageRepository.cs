using System.Data;
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

    public async Task<string?> GetCallOffMessage(long callOffId) => 
        await ExecuteQuery(CallOffQuery.GetQuery(callOffId), callOffId.ToString());

    public async Task<string?> GetCommPkgQueryMessage(long commPkgId, long documentId) =>
        await ExecuteQuery(QueryCommPkgQuery.GetQuery(commPkgId,documentId), commPkgId+","+documentId);

    public async Task<string?> GetCommPkgOperationMessage(long commPkgId) =>
        await ExecuteQuery(CommPkgOperationQuery.GetQuery(commPkgId), commPkgId.ToString());

    public async Task<string?> GetDocumentMessage(long documentId) =>
        await ExecuteQuery(DocumentQuery.GetQuery(documentId), documentId.ToString());

    public async Task<string?> GetTaskMessage(long taskId) =>
        await ExecuteQuery(TaskQuery.GetQuery(taskId), taskId.ToString());
    public async Task<string?> GetSwcrOtherReferenceMessage(string guid) =>
    await ExecuteQuery(SwcrOtherReferencesQuery.GetQuery(guid), guid);

    public async Task<string?> GetSwcrTypeMessage(string guid) =>
    await ExecuteQuery(SwcrTypeQuery.GetQuery(guid), guid);

    public async Task<string?> GetSwcrAttachmentMessage(string guid) =>
    await ExecuteQuery(SwcrAttachmentQuery.GetQuery(guid), guid);

    public async Task<string?> GetActionMessage(long actionId) =>
    await ExecuteQuery(ActionQuery.GetQuery(actionId), actionId.ToString());

    public async Task<string?> GetCommPkgTaskMessage(long commPkgId, long taskId) =>
        await ExecuteQuery(CommPkgTaskQuery.GetQuery(commPkgId, taskId), commPkgId + "," + taskId);

    public async Task<string?> GetLibraryFieldMessage(string guid) =>
        await ExecuteQuery(LibraryFieldQuery.GetQuery(guid), guid);

    public async Task<string?> GetLoopContentMessage(long loopContentId) => 
        await ExecuteQuery(LoopContentQuery.GetQuery(loopContentId),loopContentId.ToString());

    public async Task<string?> GetPipingRevisionMessage(long pipeRevisionId) =>
        await ExecuteQuery(PipingRevisionQuery.GetQuery(pipeRevisionId), pipeRevisionId.ToString());

    public async Task<string?> GetHeatTraceMessage(long id) =>
        await ExecuteQuery(HeatTraceQuery.GetQuery(id), id.ToString());

    public async Task<string?> GetPipingSpoolMessage(long pipingSpoolId) =>
        await ExecuteQuery(PipingSpoolQuery.GetQuery(pipingSpoolId), pipingSpoolId.ToString());

    public async Task<string?> GetQueryMessage(long documentId) =>
        await ExecuteQuery(QueryQuery.GetQuery(documentId), documentId.ToString());

    public async Task<string?> GetQuerySignatureMessage(long querySignatureId) =>
        await ExecuteQuery(QuerySignatureQuery.GetQuery(querySignatureId), querySignatureId.ToString());

    public async Task<string?> GetStockMessage(long stockId) =>
        await ExecuteQuery(StockQuery.GetQuery(stockId), stockId.ToString());

    public async Task<string?> GetSwcrMessage(long swcrId) =>
        await ExecuteQuery(SwcrQuery.GetQuery(swcrId), swcrId.ToString());

    public async Task<string?> GetSwcrSignatureMessage(long swcrSignatureId) =>
        await ExecuteQuery(SwcrSignatureQuery.GetQuery(swcrSignatureId), swcrSignatureId.ToString());

    public async Task<string?> GetWorkOrderChecklistMessage(long tagCheckId, long woId) =>
        await ExecuteQuery(WorkOrderChecklistsQuery.GetQuery(tagCheckId, woId), $"{tagCheckId},{woId}");

    public async Task<string?> GetWorkOrderMaterialMessage(long woId) =>
        await ExecuteQuery(WorkOrderMaterialQuery.GetQuery(woId), woId.ToString());

    public async Task<string?> GetWorkOrderCutOffMessage(long workOrderId, string cutoffWeek) =>
        await ExecuteQuery(WorkOrderCutoffQuery.GetQuery(workOrderId, cutoffWeek ), $"{workOrderId},{cutoffWeek}");

    //public async Task<string> GetWorkOrderMilestoneMessage(long woId, long milestoneId) =>
    //    await ExecuteQuery(WorkOrderMilestoneQuery.GetQuery(woId, milestoneId), $"{woId},{milestoneId}");

    private async Task<string?> ExecuteQuery(string queryString, string objectId)
    {
        var dbConnection = _context.Database.GetDbConnection();
        await using var command = dbConnection.CreateCommand();
        command.CommandText = queryString;

        var connectionWasClosed = dbConnection.State != ConnectionState.Open;
        if (connectionWasClosed)
        {
            await _context.Database.OpenConnectionAsync();
        }

        try
        {
            var result = (string?)await command.ExecuteScalarAsync();
            if (result is null)
            {
                _logger.LogError("Object/Entity with id {objectId} did not return anything", objectId);
                return null;
            }
            return result;
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
