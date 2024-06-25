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
            d.documentno as DocumentNo, 
            l.code as NotificationType, 
            d.title as Title
            from notification n
            join document d on d.document_id = n.document_id
            join project p on p.project_id = d.project_id
            left join library l on l.library_id = n.notificationtype_id
        {whereClause.clause}";

        return (query, whereClause.parameters);
    }
}
