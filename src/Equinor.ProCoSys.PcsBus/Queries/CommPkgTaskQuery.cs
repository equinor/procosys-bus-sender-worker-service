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
       '""}}' as message
        FROM CommPkg c
        INNER JOIN Project p ON c.Project_Id = p.Project_Id
        INNER JOIN ElementContent ec ON {taskId} = ec.Element_Id
        WHERE c.CommPkg_Id={commPkgId}";
        return sql;
    }

}
