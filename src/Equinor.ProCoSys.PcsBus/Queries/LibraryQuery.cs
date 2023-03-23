namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class LibraryQuery
{
    public static string GetQuery(long? libraryId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(libraryId, plant, "l", "library_id");

        return @$"select
            l.projectschema as Plant,
            l.procosys_guid as ProCoSysGuid,
            l.library_id as LibraryId,
            l.parent_id as ParentId,
            lp.procosys_guid as ParentGuid,
            l.code as Code,
            l.description as Description,
            l.isVoided as IsVoided,
            l.librarytype as Type,
            l.LAST_UPDATED as LastUpdated
        from library l
            left join library lp on l.parent_id = lp.library_id
        {whereClause}";
    }
}
