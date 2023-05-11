using System;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class CommPkgTaskQuery
{
    public static string GetQuery(long? commPkgId, long? taskId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(commPkgId, taskId, plant);

        return @$"select
            c.ProjectSchema as Plant,
            p.Title as ProjectName,
            er.procosys_guid as ProCoSysGuid,
            ec.procosys_guid as TaskGuid,
            c.procosys_guid as CommPkgGuid,
            c.CommPkgNo as CommPkgNo,
            er.Last_Updated as LastUpdated
        from ElementReference er
            join CommPkg c ON er.FromElement_Id = c.CommPkg_Id
            join Project p ON  c.Project_Id = p.Project_Id
            join ElementContent ec ON er.ToElement_Id = ec.Element_Id
        {whereClause}";
    }

    private static string CreateWhereClause(long? commPkgId, long? taskId, string? plant)
    {
        var whereClause = "";
        if (commPkgId != null && taskId != null && plant != null)
        {
            whereClause =
                $"where er.ProjectSchema='{plant}' AND er.FromElement_Id = {commPkgId} AND er.ToElement_Id={taskId} AND er.Association='Task'";
        }
        else if (plant != null)
        {
            whereClause = $"where er.ProjectSchema='{plant}' AND er.Association='Task'";
        }
        else if (commPkgId != null && taskId != null)
        {
            whereClause =
                $"where er.FromElement_Id={commPkgId} AND er.ToElement_Id = {taskId} AND er.Association='Task'";
        }
        else if (commPkgId != null || taskId != null)
        {
            throw new Exception(
                "Message can not contain partial id match, need both commPkgId and taskId to find correct db entry");
        }

        return whereClause;
    }
}
