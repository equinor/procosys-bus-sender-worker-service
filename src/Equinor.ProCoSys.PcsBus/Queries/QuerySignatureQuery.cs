﻿namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class QuerySignatureQuery
{
    public static string GetQuery(long? queryId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(queryId, plant, "q", "id");

        return @$"select
              '{{""Plant"" : ""' || q.projectschema
            ||'"", ""ProCoSysGuid"" : ""' || q.procosys_guid
            ||'"", ""PlantName"" : ""' || regexp_replace(ps.TITLE, '([""\])', '\\\1')
            ||'"", ""ProjectName"" : ""' || p.NAME
            ||'"", ""Status"" : ""' || status.code
            ||'"", ""LibraryStatusGuid"" : ""' || status.procosys_guid
            ||'"", ""QuerySignatureId"": ""'|| q.id
            ||'"", ""QueryId"": ""'|| do.document_id
            ||'"", ""QueryGuid"": ""'|| do.procosys_guid
            ||'"", ""QueryNo"": ""'|| do.documentno
            ||'"", ""SignatureRoleCode"": ""' || regexp_replace(sr.code, '([""\])', '\\\1')
            ||'"", ""FunctionalRoleCode"": ""' || regexp_replace(fr.code, '([""\])', '\\\1')
            ||'"", ""Sequence"": ""' || q.ranking
            ||'"", ""SignedByAzureOid"": ""' || p.azure_oid
            ||'"", ""FunctionalRoleDescription"": ""' || regexp_replace(fr.description, '([""\])', '\\\1')
            ||'"", ""SignedDate"": ""' || TO_CHAR(q.signedat, 'yyyy-mm-dd hh24:mi:ss')
            ||'"", ""LastUpdated"": ""' || TO_CHAR(q.last_updated, 'yyyy-mm-dd hh24:mi:ss')
            ||'""}}'
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
