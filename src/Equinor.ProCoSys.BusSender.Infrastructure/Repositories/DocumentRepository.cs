using System;
using System.Data.Common;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly BusSenderServiceContext _context;
    private readonly ILogger<DocumentRepository> _logger;

    public DocumentRepository(ILogger<DocumentRepository> logger,
        BusSenderServiceContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<string> GetQueryMessage(long documentId)
        => await ExecuteDocumentQuery(GetQueryQuery(documentId), documentId);

    public async Task<string> GetDocumentMessage(long documentId)
        => await ExecuteDocumentQuery(GetDocumentQuery(documentId), documentId);

    public async Task<string> ExecuteDocumentQuery(string queryString, long documentId)
    {
        await using var command = _context.Database.GetDbConnection().CreateCommand();
        command.CommandText = queryString;
        await _context.Database.OpenConnectionAsync();
        await using var result = await command.ExecuteReaderAsync();

        return await ExtractQueryFromResult(documentId, result);
    }

    private async Task<string> ExtractQueryFromResult(long documentId, DbDataReader result)
    {
        if (!result.HasRows)
        {
            _logger.LogError("Document with id {documentId} did not return anything", documentId);
            return "{}";
        }

        //result.ReadAsync is expected to return true here, query for 1 documentId should return 1 and only 1 row. 
        if (!await result.ReadAsync() || result[0] is DBNull)
        {
            _logger.LogError("Document with id {documentId} did not return anything", documentId);
            return "{}";
        }

        var queryResult = (string)result[0];

        //result.ReadAsync is expected to be false here, this is because there should be no more rows to read.
        if (await result.ReadAsync())
        {
            _logger.LogError("Document returned more than 1 row, this should not happen.");
        }

        return queryResult;
    }

    private static string GetQueryQuery(long documentId) =>
        @$"select
        '{{""Plant"" : ""' || q.projectschema ||
        '"", ""ProjectName"" : ""' || p.name ||
        '"", ""QueryId"" : ""'|| do.document_id ||
        '"", ""QueryNo"" : ""'|| do.documentno ||
        '"", ""Description"" : ""'|| regexp_replace(do.title , '([""\])', '\\\1') ||
        '"", ""Discipline"" : ""'|| dis.code ||
        '"", ""QueryType"" : ""'|| qt.code ||
        '"", ""CostImpactId"" : ""'||  ci.code ||
        '"", ""Consequence"" : ""'||  regexp_replace(q.CONSEQUENCE , '([""\])', '\\\1') ||
        '"", ""ProposedSolution"" : ""'|| regexp_replace(q.PROPOSEDSOLUTION , '([""\])', '\\\1') ||
        '"", ""EngineeringReply"" : ""'|| regexp_replace(q.Engineeringreply, '([""\])', '\\\1') ||
        '"", ""Milestone"" :""'|| (select code
                                       from procosys.library
                                       where library_id =
                                         (select library_id
                                          from procosys.elementfield fi_ex
                                          where fi_ex.element_id = q.document_id
                                          and EXISTS
                                            (select 1
                                            from procosys.field f
                                            where f.columnname = 'QUERY_SM'
                                            and f.field_id = fi_ex.field_id))) ||
        '"", ""ScheduleImpact"" : '|| decode(q.SCHEDULEIMPACT,'Y', 'true', 'false') ||
        ', ""PossibleWarrantyClaim"" : '|| decode(q.POSSIBLEWARRENTYCLAIM,'Y', 'true', 'false') ||
        ', ""IsVoided"" : ' || decode(e.IsVoided,'Y', 'true', 'N', 'false') ||
        ', ""RequiredDate"" : ""'|| TO_CHAR(q.REQUIREDREPLYDATE, 'yyyy-mm-dd hh24:mi:ss') ||
        '"", ""CreatedAt"" :""'|| TO_CHAR(e.CREATEDAT, 'yyyy-mm-dd hh24:mi:ss') ||
        '"", ""LastUpdated"" : ""'|| TO_CHAR(q.last_updated, 'yyyy-mm-dd hh24:mi:ss') ||
        '""}}' as message
        from query q
            join document do ON do.document_id = q.document_id
            left join project p on p.project_id = do.project_id
            join element e on e.element_id = do.document_id
            left join library dis on dis.library_id = do.discipline_id
            left join library qt on qt.library_id = q.QUERYTYPE_ID
            left join library ci ON ci.library_id = q.COSTIMPACT_ID
         where q.document_id = {documentId}";

    private static string GetDocumentQuery(long documentId) =>
        @$"select
       '{{""Plant"": ""' || d.projectschema ||
       '"", ""ProjectName"" : ""' || p.name ||
       '"", ""DocumentId"" : ""' ||d.document_id ||
       '"", ""DocumentNo"" : ""' || d.DocumentNo ||
       '"", ""Title"" : ""' || p.name ||
       '"", ""AcceptanceCode"" : ""' || apc.code ||
       '"", ""Archive"" : ""' || arc.code ||
       '"", ""AccessCode"" : ""' || acc.code ||
       '"", ""Complex"" : ""' || com.code ||
       '"", ""DocumentType"" : ""' || DT.code ||
       '"", ""DisciplineId"" : ""' || dp.code ||
       '"", ""DocumentCategory"" : ""' || dc.code ||
       '"", ""HandoverStatus"" : ""' || ho.code ||
       '"", ""RegisterType"" : ""' || RT.code ||
       '"", ""RevisionNo"" : ""' || d.revisionno ||
       '"", ""RevisionStatus"" : ""' || rev.code ||
       '"", ""ResponsibleContractor"" : ""' || res.code ||
       '"", ""LastUpdated"" : ""' || TO_CHAR(d.Last_Updated, 'yyyy-mm-dd hh24:mi:ss') ||
       '"", ""RevisionDate"" : ""' || TO_CHAR(d.Revisiondate, 'yyyy-mm-dd hh24:mi:ss') ||
       '""}}' as message
        from document d
            left join project p on p.project_id = d.project_id
            left join library RT on RT.library_id = d.register_id
            left join library DT on DT.library_id = d.documenttype_id
            left join library apc on apc.library_id = d.acceptancecode_id
            left join library dc on dc.library_id = d.documentcategory_id
            left join library arc on arc.library_id = d.archive_id
            left join library dp on dp.library_id = d.discipline_id
            left join library rev on rev.library_id = d.revisionstatus_id
            left join library ho on ho.library_id = d.handoverstatus_id
            left join library res on res.library_id = d.responsiblecontractor_id
            left join library acc on acc.library_id = d.accesscode_id
            left join library com on com.library_id = d.complex_id
        where d.document_id = {documentId}";
}
