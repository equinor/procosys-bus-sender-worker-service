namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class SwcrTypeQuery
{
    public static string GetQuery(string swcrTypeGuid, string plant= null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClauseForGuid(swcrTypeGuid, plant, "elr", "procosys_guid");

        return @$"select
           '{{""Plant"": ""' || elr.ProjectSchema ||
           '"", ""ProCoSysGuid"" : ""' || elr.PROCOSYS_GUID ||
           '"", ""LibraryGuid"" : ""' || l.PROCOSYS_GUID ||
           '"", ""SwcrGuid"" : ""' || s.PROCOSYS_GUID ||
           '"", ""Code"" : ""' || l.CODE ||
           '"", ""LastUpdated"" : ""' || TO_CHAR(elr.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss') ||
           '""}}' as message
            FROM ELEMENTLIBRELATION elr
            INNER JOIN SWCR s ON s.SWCR_ID=elr.ELEMENT_ID
            INNER JOIN LIBRARY l ON elr.LIBRARY_ID=l.LIBRARY_ID
            {whereClause}";
    }
}
