using System;
using System.Linq;
using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public static class QueryHelper
{
    public static (string clause, DynamicParameters parameters) CreateWhereClause(long? id, string? plant, string shortHand, string? idColumn)
    {
        var whereClause = "";
        var parameters = new DynamicParameters();

        if (id.HasValue && plant != null)
        {
            whereClause = $"where {shortHand}.ProjectSchema=:Plant AND {shortHand}.{idColumn}=:Id";
            parameters.Add(":Plant", plant);
            parameters.Add(":Id", id);
        }
        else if (plant != null)
        {
            whereClause = $"where {shortHand}.ProjectSchema=:Plant";
            parameters.Add(":Plant", plant);
        }
        else if (id.HasValue)
        {
            whereClause = $"where {shortHand}.{idColumn}=:Id";
            parameters.Add(":Id", id);
        }

        return (whereClause, parameters);
    }

    public static (string clause, DynamicParameters parameters) CreateWhereClause(string? guid, string? plant, string shortHand, string idColumn)
    {
        var whereClause = "";
        var parameters = new DynamicParameters();

        if (guid != null && plant != null)
        {
            whereClause = $"where {shortHand}.ProjectSchema=:Plant AND {shortHand}.{idColumn}=hextoraw(:Guid)";
            parameters.Add(":Plant", plant);
            parameters.Add(":Guid", guid);
        }
        else if (plant != null)
        {
            whereClause = $"where {shortHand}.ProjectSchema=:Plant";
            parameters.Add(":Plant", plant);
        }
        else if (guid != null)
        {
            whereClause = $"where {shortHand}.{idColumn}=hextoraw(:Guid)";
            parameters.Add(":Guid", guid);
        }

        return (whereClause, parameters);
    }

    public static void DetectFaultyPlantInput(string? plant)
    {
        if (plant != null && plant.Any(char.IsWhiteSpace))
        {
            //To detect potential Sql injection 
            throw new Exception("plant should not contain spaces");
        }
    }
}
