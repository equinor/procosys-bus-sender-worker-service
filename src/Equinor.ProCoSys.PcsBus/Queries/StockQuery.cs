﻿namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class StockQuery
{
    public static string GetQuery(string schema)
    {
        return @$"select
            '{{""Plant"" : ""' || s.projectschema || 
            '"", ""StockId"" : ""' || s.id || 
            '"", ""StockNo"" : ""' || regexp_replace(s.stockno, '([""\])', '\\\1')  || 
            '"", ""Description"" : ""' || regexp_replace(s.description, '([""\])', '\\\1') || 
            '"", ""LastUpdated"" : ""' || TO_CHAR(s.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss')  ||
            '""}}'  as message
            from stock s
            where s.projectschema = '{schema}'";
    }
}
