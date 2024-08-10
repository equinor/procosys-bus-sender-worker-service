using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class NotificationCommPkgBoundaryQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string? boundaryDocumentGuid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(boundaryDocumentGuid, plant, "bd", "procosys_guid");
        const string Notification = "NOTIFICATION";

        var query = @$"select  
                bd.procosys_guid as ProCoSysGuid,
                bd.projectschema as Plant, 
                p.name as ProjectName, 
                p.procosys_guid as ProjectGuid,       
                d.procosys_guid as NotificationGuid,
                c.procosys_guid as CommPkgGuid,
                bd.last_updated as LastUpdated,
                'BOUNDARY' as RelationshipType
            from boundarydocument bd
                join document d on d.document_id = bd.document_id
                join commpkg c on c.commpkg_id = bd.commpkg_id
                join library l on l.library_id = d.register_id and l.code = '{Notification}'
                join project p on p.project_id = c.project_id
        {whereClause.clause}";

        return (query, whereClause.parameters);
    }
}
