namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class SwcrTypeQuery
{
    public static string GetQuery(string swcrTypeGuid, string? plant= null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClauseForGuid(swcrTypeGuid, plant, "elr", "procosys_guid");

        return @$"select
            elr.ProjectSchema as Plant,
            elr.PROCOSYS_GUID as ProCoSysGuid,
            l.PROCOSYS_GUID as LibraryGuid,
            s.PROCOSYS_GUID as SwcrGuid,
            l.CODE as Code,
            elr.LAST_UPDATED as LastUpdated
        from ELEMENTLIBRELATION elr
            join SWCR s ON s.SWCR_ID = elr.ELEMENT_ID
            join LIBRARY l ON elr.LIBRARY_ID = l.LIBRARY_ID
        {whereClause}";
    }
}
