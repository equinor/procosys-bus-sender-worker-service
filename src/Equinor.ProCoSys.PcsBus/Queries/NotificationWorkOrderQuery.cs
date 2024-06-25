using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class NotificationWorkOrderQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? documentId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(documentId, plant, "nwo", "document_id");

        var query = @$"select 
            nwo.projectschema as Plant, 
            nwo.procosys_guid as ProCoSysGuid, 
            p.name as ProjectName, 
            d.procosys_guid as NotificationGuid,
            wo.procosys_guid as WorkOrderGuid
            from notification_wo nwo
            join notification n on n.document_id = nwo.document_id
            join wo on wo.wo_id = nwo.wo_id
            join document d on d.document_id = nwo.document_id
            join project p on p.project_id = d.project_id
        {whereClause.clause}";

        return (query, whereClause.parameters);
    }
}
