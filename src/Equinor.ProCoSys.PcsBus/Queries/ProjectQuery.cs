namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class ProjectQuery
{
    public static string GetQuery(long? projectId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(projectId, plant, "p", "project_id");

        return @$"select
            '{{""Plant"" : ""' || p.projectschema ||
            '"", ""ProCoSysGuid"" : ""' || p.procosys_guid ||
            '"", ""ProjectName"" : ""' || p.NAME || 
            '"", ""IsClosed"" : ' || decode(p.IsVoided,'Y', 'true', 'N', 'false') || 
            ', ""Description"" : ""' || REPLACE(REPLACE(p.DESCRIPTION,'\','\\'),'""','\""') || 
            '""}}'  as message
        from project p
        {whereClause}";
    }
}
