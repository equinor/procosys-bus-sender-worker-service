using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class SwcrTypeQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string swcrTypeGuid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(swcrTypeGuid, plant, "elr", "procosys_guid");

        var query = @$"select
            elr.ProjectSchema as Plant,
            elr.procosys_guid as ProCoSysGuid,
            l.procosys_guid as LibraryGuid,
            s.procosys_guid as SwcrGuid,
            l.code as Code,
            elr.LAST_UPDATED as LastUpdated
        from ELEMENTLIBRELATION elr
            join SWCR s ON s.SWCR_ID = elr.ELEMENT_ID
            join LIBRARY l ON elr.LIBRARY_ID = l.LIBRARY_ID
        {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }
}
