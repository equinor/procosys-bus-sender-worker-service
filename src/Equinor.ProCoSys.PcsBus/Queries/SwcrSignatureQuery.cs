using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class SwcrSignatureQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? swcrSignatureId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(swcrSignatureId, plant, "sign", "swcrsignature_id");

        var query = @$"select
            sign.projectschema as Plant,
            sign.procosys_guid as ProCoSysGuid,
            sign.swcrsignature_id as SwcrSignatureId,
            p.NAME as ProjectName,
            s.swcrno as SwcrNo,
            s.procosys_guid as SwcrGuid,
            sr.code as SignatureRoleCode,
            sr.description as SignatureRoleDescription,
            sign.ranking as Sequence,
            p.azure_oid as SignedByAzureOid,
            fr.code as FunctionalRoleCode,
            fr.description as FunctionalRoleDescription,
            sign.signedat as SignedDate,
            sign.last_updated as LastUpdated
        from swcrsignature sign
            join swcr s on s.swcr_id = sign.swcr_id
            join projectschema ps ON ps.projectschema = sign.projectschema
            join library sr ON sr.library_id = sign.signaturerole_id
            join project p ON p.project_id = s.project_id
            left join person p ON p.person_id = sign.signedby_id
            left join library fr On fr.library_id = sign.functionalrole_id
        {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }
}
