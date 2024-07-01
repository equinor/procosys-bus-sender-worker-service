using System;
using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class NotificationWorkOrderQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string? workOrderDocumentGuid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(workOrderDocumentGuid, plant, "wod", "procosys_guid");
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
}
