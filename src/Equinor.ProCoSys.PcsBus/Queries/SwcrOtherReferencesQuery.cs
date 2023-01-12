namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class SwcrOtherReferencesQuery
{
    public static string GetQuery(string swcrOtherReferencesGuid, string plant= null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClauseForGuid(swcrOtherReferencesGuid, plant, "slr", "procosys_guid");

        return @$"select
           '{{""Plant"": ""' || slr.ProjectSchema ||
           '"", ""ProCoSysGuid"" : ""' || slr.PROCOSYS_GUID ||
           '"", ""LibraryGuid"" : ""' || l.PROCOSYS_GUID ||
           '"", ""SwcrGuid"" : ""' || s.PROCOSYS_GUID ||
           '"", ""Code"" : ""' || l.CODE ||
           '"", ""Description"" : ""' || regexp_replace(slr.DESCRIPTION, '([""\])', '\\\1') ||
           '"", ""LastUpdated"" : ""' || TO_CHAR(slr.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss') ||
           '""}}' as message
            FROM SWCRLIBRARYREFERENCE slr
            INNER JOIN SWCR s ON s.SWCR_ID=slr.SWCR_ID
            INNER JOIN LIBRARY l ON slr.LIBRARY_ID=l.LIBRARY_ID
            {whereClause}";
    }
}
