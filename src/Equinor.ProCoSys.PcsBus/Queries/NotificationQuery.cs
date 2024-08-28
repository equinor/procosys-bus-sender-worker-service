using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class NotificationQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? documentId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(documentId, plant, "n", "document_id");

        var query = @$"select d.procosys_guid as ProCoSysGuid, 
            n.projectschema as Plant, 
            p.name as ProjectName, 
            d.document_id as NotificationId,
            d.documentno as NotificationNo, 
            doctype.code as DocumentType,
            resp.code as ResponsibleContractor,
            e.createdAt as CreatedAt,
            n.last_updated as LastUpdated,
            l.code as NotificationType, 
            d.title as Title
            from notification n
            join document d on d.document_id = n.document_id
            join project p on p.project_id = d.project_id
            join element e on e.element_id = d.document_id
            left join library l on l.library_id = n.notificationtype_id
            left join library doctype on doctype.library_id = d.documenttype_id
            left join library resp on resp.library_id = d.responsiblecontractor_id
        {whereClause.clause}";

        return (query, whereClause.parameters);
    }
}
