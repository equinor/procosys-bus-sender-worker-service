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
}
