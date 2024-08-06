using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class NotificationCommPkgOtherQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string? workOrderDocumentGuid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(workOrderDocumentGuid, plant, "e", "procosys_guid");
        const string Notification = "NOTIFICATION";

        var query = @$"select  
            e.procosys_guid as ProCoSysGuid, 
            e.projectschema as Plant, 
            p.name as ProjectName, 
            p.procosys_guid as ProjectGuid,       
            d.procosys_guid as NotificationGuid,
            c.procosys_guid as CommPkgGuid,
            e.last_updated as LastUpdated
        from elementreference e
            join document d on d.document_id = e.toelement_id
            join commpkg c on c.commpkg_id = e.fromelement_id
            join library l on l.library_id = d.register_id and l.code = '{Notification}'
            join project p on p.project_id = c.project_id
        {whereClause.clause}";

        return (query, whereClause.parameters);
    }
}
