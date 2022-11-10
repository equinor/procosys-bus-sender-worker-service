namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class StockQuery
{
    public static string GetQuery(long? stockId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(stockId, plant, "s", "id");

        return @$"select
            '{{""Plant"" : ""' || s.projectschema ||
            '"", ""ProCoSysGuid"" : ""' || s.procosys_guid ||
            '"", ""StockId"" : ""' || s.id || 
            '"", ""StockNo"" : ""' || regexp_replace(s.stockno, '([""\])', '\\\1')  || 
            '"", ""Description"" : ""' || regexp_replace(s.description, '([""\])', '\\\1') || 
            '"", ""LastUpdated"" : ""' || TO_CHAR(s.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss')  ||
            '""}}'  as message
        from stock s
        {whereClause}";
    }
}
