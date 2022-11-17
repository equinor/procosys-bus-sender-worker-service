using System;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class CommPkgTaskQuery
{
    public static string GetQuery(long? commPkgId, long? taskId, string plant= null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(commPkgId, taskId, plant);

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
        {whereClause}";
        return sql;
    }

    private static string CreateWhereClause(long? commPkgId, long? taskId, string plant)
    {
        var whereClause = "";
        if (commPkgId != null && taskId != null && plant != null)
        {
            whereClause = $"WHERE er.ProjectSchema='{plant}' AND er.FromElement_Id={commPkgId} AND er.ToElement_Id={taskId} AND er.Association='Task'";
        }
        else if (plant != null)
        {
            whereClause = $"WHERE er.ProjectSchema='{plant}' AND er.Association='Task'";
        }
        else if (commPkgId != null && taskId != null)
        {
            whereClause = $"WHERE er.FromElement_Id={commPkgId} AND er.ToElement_Id={taskId} AND er.Association='Task'";
        }
        else if (commPkgId != null || taskId != null)
        {
            throw new Exception("Message can not contain partial id match, need both commPkgId and taskId to find correct db entry");
        }

        return whereClause;
    }
}
