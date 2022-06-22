﻿using System;
using System.Linq;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class QueryQuery
{
    public static string GetQuery(long? documentId, string plant = null)
    {
        if (plant != null && plant.Any(char.IsWhiteSpace))
        {
            //To detect potential Sql injection 
            throw new Exception("plant should not contain spaces");
        }

        var whereClause = CreateWhereClause(documentId, plant);
        return @$"select
        '{{""Plant"" : ""' || q.projectschema ||
        '"", ""ProjectName"" : ""' || p.name ||
        '"", ""QueryId"" : ""'|| do.document_id ||
        '"", ""QueryNo"" : ""'|| regexp_replace(do.documentno, '([""\])', '\\\1') ||
        '"", ""Title"" : ""'|| regexp_replace(do.title , '([""\])', '\\\1') ||
        '"", ""Discipline"" : ""'|| dis.code ||
        '"", ""QueryType"" : ""'|| qt.code ||
        '"", ""CostImpact"" : ""'||  ci.code ||
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
         {whereClause}";

    }

    private static string CreateWhereClause(long? documentId, string plant)
    {
        var whereClause = "";
        if (documentId != null && plant != null)
        {
            whereClause = $"where q.projectschema = '{plant}' and q.document_id = {documentId}";
        }
        else if (plant != null)
        {
            whereClause = $"where q.projectschema = '{plant}'";
        }
        else if (documentId != null)
        {
            whereClause = $"where q.document_id = {documentId}";
        }

        return whereClause;
    }
}
