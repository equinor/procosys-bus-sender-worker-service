using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class QuerySignatureQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? queryId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(queryId, plant, "q", "id");

        var query = @$"select
            q.projectschema as Plant,
            q.procosys_guid as ProCoSysGuid,
            ps.TITLE as PlantName,
            pr.NAME as ProjectName,
            status.code as Status,
            status.procosys_guid as LibraryStatusGuid,
            q.id as QuerySignatureId,
            do.document_id as QueryId,
            do.procosys_guid as QueryGuid,
            do.documentno as QueryNo,
            sr.code as SignatureRoleCode,
            fr.code as FunctionalRoleCode,
            q.ranking as Sequence,
            p.azure_oid as SignedByAzureOid,
            fr.description as FunctionalRoleDescription,
            q.signedat as SignedDate,
            q.last_updated as LastUpdated
        from querysignature q
            join document DO ON do.document_id = q.DOCUMENT_ID
            join projectschema ps on ps.projectschema = q.projectschema
            join element em on em.element_id = do.document_id
            left join project pr ON pr.project_id = do.project_id
            left join library sr ON sr.library_id = q.signaturerole_id
            left join person p ON p.person_id = q.signedby_id
            left join library fr On fr.library_id = q.functionalrole_id
            left join library status on status.library_id = q.status_id
        {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }
}
