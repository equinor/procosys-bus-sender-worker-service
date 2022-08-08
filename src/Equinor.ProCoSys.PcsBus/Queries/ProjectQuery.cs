namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class ProjectQuery
{
    public static string GetQuery(long? projectId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(projectId, plant, "p", "project_id");

        return @$"select
            '{{""Plant"" : ""' || p.projectschema || 
            '"", ""ProjectName"" : ""' || p.NAME || 
            '"", ""IsClosed"" : ' || (case when p.ISVOIDED = 'Y' then 'true' else 'false' end) || 
            '"", ""Description"" : ""' || REPLACE(REPLACE(p.DESCRIPTION,'\','\\'),'""','\""') || 
            '""}}'  as message
        from project p
        {whereClause}";
    }
}
