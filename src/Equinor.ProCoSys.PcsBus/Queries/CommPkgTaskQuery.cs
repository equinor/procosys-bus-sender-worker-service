using System;
using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class CommPkgTaskQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? commPkgId, long? taskId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(commPkgId, taskId, plant);

        var queryString = @$"select
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
        {whereClause.clause}";
        
        return (queryString,whereClause.parameters);
    }

    private static (string clause, DynamicParameters parameters) CreateWhereClause(long? commPkgId, long? taskId, string? plant)
    {
        var whereClause = "";
        var parameters = new DynamicParameters();

        if (commPkgId.HasValue && taskId.HasValue)
        {
            whereClause += "where er.FromElement_Id=:CommPkgId AND er.ToElement_Id=:TaskId";
            parameters.Add(":CommPkgId", commPkgId);
            parameters.Add(":TaskId", taskId);
        
            if (plant != null)
            {
                whereClause += " AND er.ProjectSchema=:Plant";
                parameters.Add(":Plant", plant);
            }
        
            whereClause += " AND er.Association='Task'";
        }
        else if (plant != null)
        {
            whereClause = "where er.ProjectSchema=:Plant AND er.Association='Task'";
            parameters.Add(":Plant", plant);
        }
        else if (commPkgId.HasValue || taskId.HasValue)
        {
            throw new Exception("Message cannot contain partial id match, need both commPkgId and taskId to find correct db entry");
        }

        return (whereClause, parameters);
    }
}
