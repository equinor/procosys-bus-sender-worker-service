namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class QuerySignatureQuery
{
    public static string GetQuery(long? queryId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(queryId, plant, "q", "id");

        return @$"select
            q.projectschema as Plant,
            q.procosys_guid as ProCoSysGuid,
            ps.TITLE as PlantName,
            p.NAME as ProjectName,
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
            join project p ON p.project_id = do.project_id
            join projectschema ps on ps.projectschema = q.projectschema
            join element em on em.element_id = do.document_id
            join library dlib on dlib.library_id = do.discipline_id
            join library sr ON sr.library_id = q.signaturerole_id
            left join person p ON p.person_id = q.signedby_id
            left join library fr On fr.library_id = q.functionalrole_id
            left join library status on status.library_id = q.status_id
        {whereClause}";
    }
}
