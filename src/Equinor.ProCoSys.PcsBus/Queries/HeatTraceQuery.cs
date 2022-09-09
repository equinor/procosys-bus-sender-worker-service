using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class HeatTraceQuery
{
    public static string GetQuery(long? heatTraceId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(heatTraceId, plant, "ht", "id");

        return @$"select
        '{{""Plant"" : ""' || ht.projectschema ||
        '"", ""HeatTraceId"" : ""' || ht.id ||
        '"", ""CableId"" : ""' || ht.cable_id ||
        '"", ""TagId"" : ""' || ht.tag_id ||
        '"", ""SpoolNo"" : ""' || ht.spoolno ||
        '"", ""LastUpdated"" : ""' || TO_CHAR(ht.last_updated, 'yyyy-mm-dd hh24:mi:ss') ||
        '""}}' as message
        from htjboxcableservice ht
            join pipingrevision pr on pr.pipingrevision_id = pt.pipingrevision_id      
            join pipetestlibrary ptl on ptl.pipetestlibrary_id = pt.pipetestlibrary_id  
        {whereClause}";
    }
}
