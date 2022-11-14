namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class CommPkgTaskQuery
{
    public static string GetQuery(long? commPkgId, long? taskId, string plant= null)
    {
        DetectFaultyPlantInput(plant);
        
        var sql = @$"select
       '{{""Plant"": ""' || c.ProjectSchema ||
       '"", ""ProjectName"" : ""' || p.Title ||
       '"", ""TaskGuid"" : ""' || ec.ProCoSys_Guid ||
       '"", ""CommPkgGuid"" : ""' || c.Procosys_Guid ||
       '"", ""CommPkgNo"" : ""' || c.CommPkgNo ||
       '"", ""LastUpdated"" : ""' || TO_CHAR(er.Last_Updated, 'yyyy-mm-dd hh24:mi:ss') ||
       '""}}' as message
        FROM ElementReference er
        INNER JOIN CommPkg c ON er.FromElement_Id=c.CommPkg_Id
        INNER JOIN Project p ON  c.Project_Id = p.Project_Id
        INNER JOIN ElementContent ec ON er.ToElement_Id = ec.Element_Id
        WHERE FromElement_Id={commPkgId} and ToElement_Id={taskId} AND Association='Task'";
        return sql;
    }

}
