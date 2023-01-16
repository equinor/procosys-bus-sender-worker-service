using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Handlers;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Queries;
using Equinor.ProCoSys.PcsServiceBus.Topics;
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


    public async Task<string> GetCommPkgQueryMessage(long commPkgId, long documentId) =>
        await ExecuteQuery(QueryCommPkgQuery.GetQuery(commPkgId,documentId), commPkgId+","+documentId);

    public async Task<string> GetCommPkgOperationMessage(long commPkgId) =>
        await ExecuteQuery(CommPkgOperationQuery.GetQuery(commPkgId), commPkgId.ToString());

    public async Task<string> GetDocumentMessage(long documentId) =>
        await ExecuteQuery(DocumentQuery.GetQuery(documentId), documentId.ToString());

    public async Task<string> GetTaskMessage(long taskId) =>
        await ExecuteQuery(TaskQuery.GetQuery(taskId), taskId.ToString());

    public async Task<string> GetActionMessage(long actionId) =>
    await ExecuteQuery(ActionQuery.GetQuery(actionId), actionId.ToString());

    public async Task<string> GetCommPkgTaskMessage(long commPkgId, long taskId) =>
        await ExecuteQuery(CommPkgTaskQuery.GetQuery(commPkgId, taskId), commPkgId + "," + taskId);

    public async Task<string> GetCommPkgPriorityMessage(string guid) =>
        await ExecuteQuery(CommPkgPriorityQuery.GetQuery(guid), guid);

    public async Task<string> GetLoopContentMessage(long loopContentId) => 
        await ExecuteQuery(LoopContentQuery.GetQuery(loopContentId),loopContentId.ToString());

    public async Task<string> GetPipingRevisionMessage(long pipeRevisionId) =>
        await ExecuteQuery(PipingRevisionQuery.GetQuery(pipeRevisionId), pipeRevisionId.ToString());

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


    //private async Task<IEnumerable<ChecklistEvent>> ExecuteQueryForEvent(string queryString, string objectId)
    //{
    //    await using var connection = _context.Database.GetDbConnection();
    //    if (_context.Database.GetDbConnection().State != ConnectionState.Open)
    //    {
    //        await _context.Database.OpenConnectionAsync();
    //    }
        
    //    var checklists = connection.Query<ChecklistEvent>(queryString).ToList();
    //    if (checklists.Count == 0)
    //    {
    //        _logger.LogError("Object/Entity with id {objectId} did not return anything", objectId);
    //        return null;
    //    }
    //    return checklists;
    //}

    private async Task<string> ExecuteQuery(string queryString, string objectId)
    {
        await using var command = _context.Database.GetDbConnection().CreateCommand();
        command.CommandText = queryString;
        if( _context.Database.GetDbConnection().State != ConnectionState.Open)
        {
            await _context.Database.OpenConnectionAsync();
        }

        var result = (string)await command.ExecuteScalarAsync();

        if (result is null)
        {
            _logger.LogError("Object/Entity with id {objectId} did not return anything", objectId);
            return null;
        }
        return result;
    }



}
