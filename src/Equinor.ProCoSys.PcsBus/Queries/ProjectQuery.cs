using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class ProjectQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? projectId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(projectId, plant, "p", "project_id");

        var query = @$"select
            p.projectschema as Plant,
            p.procosys_guid as ProCoSysGuid,
            p.NAME as ProjectName,
            p.IsVoided as IsClosed,
            p.DESCRIPTION as Description,
            p.last_updated as LastUpdated
        from project p
        {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }
}
