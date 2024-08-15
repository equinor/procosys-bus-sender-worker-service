using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class NotificationSignatureQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string? notificationSignatureGuid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(notificationSignatureGuid, plant, "ns", "procosys_guid");

        var query = $@"select
                ns.procosys_guid as ProCoSysGuid,
                ns.projectschema as Plant, 
                p.name as ProjectName, 
                p.procosys_guid as ProjectGuid, 
                n.procosys_guid as NotificationGuid,
                sr.description as SignatureRole,
                ns.sequence as Sequence,
                ( CASE
                    WHEN (
                    SELECT
                        l.code
                    FROM
                        definelibraryfield dlf
                        JOIN field f ON dlf.field_id = f.field_id
                                        AND f.columnname = 'NOTIFICATION_SET_SIGNATURE_STATUS'
                        JOIN libraryfield lf ON dlf.id = lf.definelibraryfield_id
                        JOIN library l ON lf.value_id = l.library_id
                    WHERE
                        dlf.librarytype = 'NOTIFICATION_SIGNATURE_ROLE'
                        AND   dlf.isvoided = 'N'
                        AND   lf.library_id = SIGNATUREROLE_ID
                        ) = 'Y' 
                            AND SIGNEDAT IS NOT NULL THEN 'Accepted'
                        ELSE NULL
                    END ) as Status,
                sp.azure_oid as SignerPersonOid,
                fr.code as SignerFunctionalRoleCode,
                sb.azure_oid as SignedByOid,
                ns.signedat as SignedAt,
                ns.last_updated as LastUpdated
            from notification_signature ns
                join document d on d.document_id = ns.document_id
                join project p on p.project_id = d.project_id
                join library sr on ns.signaturerole_id = sr.library_id
                join notification n on ns.document_id = n.document_id
                left join library st on ns.status_id = st.library_id
                left join person sp on sp.person_id = ns.signerperson_id
                left join library fr on ns.signerfunctionalrole_id = fr.library_id
                left join person sb on ns.signedby_id = sb.person_id
        { whereClause.clause}";

        return (query, whereClause.parameters);
    }
}
