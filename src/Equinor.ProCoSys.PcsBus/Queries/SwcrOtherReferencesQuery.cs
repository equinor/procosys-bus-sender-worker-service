namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class SwcrOtherReferencesQuery
{
    public static string GetQuery(string swcrOtherReferencesGuid, string? plant= null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClauseForGuid(swcrOtherReferencesGuid, plant, "slr", "procosys_guid");

        return @$"select
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
        {whereClause}";
    }
}
