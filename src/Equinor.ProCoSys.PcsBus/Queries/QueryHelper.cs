using System;
using System.Linq;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public static class QueryHelper
{
    public static string CreateWhereClause(long? id, string? plant, string shortHand, string? idColumn)
    {
        var whereClause = "";
        if (id != null && plant != null)
        {
            whereClause = $"where {shortHand}.projectschema = '{plant}' and {shortHand}.{idColumn} = {id}";
        }
        else if (plant != null)
        {
            whereClause = $"where {shortHand}.projectschema = '{plant}'";
        }
        else if (id != null)
        {
            whereClause = $"where {shortHand}.{idColumn} = {id}";
        }

        return whereClause;
    }

    public static string CreateWhereClauseForGuid(string? guid, string? plant, string shortHand, string idColumn)
    {
        var whereClause = "";
        if (guid != null && plant != null)
        {
            whereClause = $"where {shortHand}.projectschema = '{plant}' and {shortHand}.{idColumn} = '{guid}'";
        }
        else if (plant != null)
        {
            whereClause = $"where {shortHand}.projectschema = '{plant}'";
        }
        else if (guid != null)
        {
            whereClause = $"where {shortHand}.{idColumn} = '{guid}'";
        }

        return whereClause;
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
