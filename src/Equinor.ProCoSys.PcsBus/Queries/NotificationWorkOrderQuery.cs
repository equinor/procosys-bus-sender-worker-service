using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class NotificationWorkOrderQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? documentId, long? workOrderId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(documentId, workOrderId, plant);
        const string Notification = "NOTIFICATION";

        var query = @$"select  
            wod.procosys_guid as ProCoSysGuid, 
            wod.projectschema as Plant, 
            p.name as ProjectName, 
            p.procosys_guid as ProjectGuid,       
            d.procosys_guid as NotificationGuid,
            wo.procosys_guid as WorkOrderGuid,
            wod.last_updated as LastUpdated
        from wo_document wod
            join document d on d.document_id = wod.document_id
            join wo wo on wo.wo_id = wod.wo_id
            join library l on l.library_id = d.register_id and l.code = '{Notification}'
            join project p on p.project_id = wo.project_id
        {whereClause.clause}";

        return (query, whereClause.parameters);
    }

    private static (string clause, DynamicParameters parameters) CreateWhereClause(long? documentId, long? workOrderId, string? plant)
    {
        var whereClause = "";
        var parameters = new DynamicParameters();

        if (documentId.HasValue && workOrderId.HasValue)
        {
            whereClause = "where wod.wo_id=:WorkOrderId AND wod.document_id=:DocumentId";
            parameters.Add(":WorkOrderId", workOrderId);
            parameters.Add(":DocumentId", documentId);

            if (plant != null)
            {
                whereClause += " AND wod.projectschema=:Plant";
                parameters.Add(":Plant", plant);
            }
        }
        else if (plant != null)
        {
            whereClause = "where wod.projectschema=:Plant";
            parameters.Add(":Plant", plant);
        }

        return (whereClause, parameters);
    }
}
