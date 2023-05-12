using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class StockQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? stockId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(stockId, plant, "s", "id");

        var query = @$"select
            s.projectschema as Plant,
            s.procosys_guid as ProCoSysGuid,
            s.id as StockId,
            s.stockno as StockNo,
            s.description as Description,
            s.LAST_UPDATED as LastUpdated
        from stock s
        {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }
}
