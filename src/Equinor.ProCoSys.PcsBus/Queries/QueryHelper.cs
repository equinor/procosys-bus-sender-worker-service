using System;
using System.Linq;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public  static class QueryHelper
{
    public static void DetectFaultyPlantInput(string plant)
    {
        if (plant != null && plant.Any(char.IsWhiteSpace))
        {
            //To detect potential Sql injection 
            throw new Exception("plant should not contain spaces");
        }
    }

    public static string CreateWhereClause(long? loopTagId, string plant, string shortHand, string idColumn)
    {
        var whereClause = "";
        if (loopTagId != null && plant != null)
        {
            whereClause = $"where {shortHand}.projectschema = '{plant}' and {shortHand}.{idColumn} = {loopTagId}";
        }
        else if (plant != null)
        {
            whereClause = $"where {shortHand}.projectschema = '{plant}'";
        }
        else if (loopTagId != null)
        {
            whereClause = $"where {shortHand}.{idColumn} = {loopTagId}";
        }

        return whereClause;
    }
}
