namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class LibraryQuery
{
    public static string GetQuery(long? libraryId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(libraryId, plant, "l", "library_id");

        return @$"select
            '{{""Plant"" : ""' || l.projectschema ||
            '"", ""ProCoSysGuid"" : ""' || l.procosys_guid ||
            '"", ""LibraryId"" : ""' || l.library_id ||
            '"", ""ParentId"" : ""' || regexp_replace(l.parent_id, '([""\])', '\\\1') ||
            '"", ""ParentGuid"" : ""' || lp.procosys_guid ||
            '"", ""Code"" : ""' || regexp_replace(l.code, '([""\])', '\\\1') ||
            '"", ""Description"" : ""' || regexp_replace(l.description, '([""\])', '\\\1') ||
            '"", ""IsVoided"" : ' || decode(l.isVoided,'Y', 'true', 'N', 'false') ||
            ', ""Type"" : ""' || regexp_replace(l.librarytype, '([""\])', '\\\1') ||
            '"", ""LastUpdated"" : ""' || TO_CHAR(l.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss') ||
            '""}}'  as message
            from library l
            left join library lp on l.parent_id=lp.library_id
            {whereClause}";
    }
}
