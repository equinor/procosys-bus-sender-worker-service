namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class StockQuery
{
    public static string GetQuery(long? stockId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(stockId, plant, "s", "id");

        return @$"select
            s.projectschema as Plant,
            s.procosys_guid as ProCoSysGuid,
            s.id as StockId,
            s.stockno as StockNo,
            s.description as Description,
            s.LAST_UPDATED as LastUpdated
        from stock s
        {whereClause}";
    }
}
