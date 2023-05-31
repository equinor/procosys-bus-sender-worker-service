using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class SwcrOtherReferenceQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string? swcrOtherReferencesGuid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(swcrOtherReferencesGuid, plant, "slr", "procosys_guid");

        var query = @$"select
            slr.ProjectSchema as Plant,
            slr.PROCOSYS_GUID as ProCoSysGuid,
            l.PROCOSYS_GUID as LibraryGuid,
            s.PROCOSYS_GUID as SwcrGuid,
            l.CODE as Code,
            slr.DESCRIPTION as Description,
            slr.LAST_UPDATED as LastUpdated
        from SWCRLIBRARYREFERENCE slr
                join SWCR s ON s.SWCR_ID = slr.SWCR_ID
                join LIBRARY l ON slr.LIBRARY_ID = l.LIBRARY_ID
        {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }
}
