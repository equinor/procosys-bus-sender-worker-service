namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class QueryQuery
{
    public static string GetQuery(long? documentId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(documentId, plant, "q", "document_id");

        return @$"select
            q.projectschema as Plant,
            do.procosys_guid as ProCoSysGuid,
            p.name as ProjectName,
            do.document_id as QueryId,
            do.documentno as QueryNo,
            do.title as Title,
            dis.code as DisciplineCode,
            qt.code as QueryType,
            ci.code as CostImpact,
            q.Description as Description,
            q.CONSEQUENCE as Consequence,
            q.PROPOSEDSOLUTION as ProposedSolution,
            q.Engineeringreply as EngineeringReply,
            (select code
                from library
                where library_id =
                    (select library_id
                    from elementfield fi_ex
                    where fi_ex.element_id = q.document_id
                    and EXISTS
                        (select 1
                        from field f
                        where f.columnname = 'QUERY_SM'
                        and f.field_id = fi_ex.field_id))) as Milestone,
            q.SCHEDULEIMPACT as ScheduleImpact,
            q.POSSIBLEWARRENTYCLAIM as PossibleWarrantyClaim,
            e.IsVoided as IsVoided,
            q.REQUIREDREPLYDATE as RequiredDate,
            e.CREATEDAT as CreatedAt,
            q.last_updated as LastUpdated
        from query q
            join document do ON do.document_id = q.document_id
            left join project p on p.project_id = do.project_id
            join element e on e.element_id = do.document_id
            left join library dis on dis.library_id = do.discipline_id
            left join library qt on qt.library_id = q.QUERYTYPE_ID
            left join library ci ON ci.library_id = q.COSTIMPACT_ID
        {whereClause}";
    }
}
