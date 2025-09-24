using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class LibraryToLibraryQuery
{
    public static (string query, DynamicParameters parameters) GetQuery(string? guid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(guid, plant, "ll", "procosys_guid");

        var query = @$"select
            ll.projectschema as Plant,
            ll.procosys_guid as ProCoSysGuid,
            ll.role as Role,
            ll.association as Association,
            l2.procosys_guid as LibraryGuid,
            l.procosys_guid as RelatedLibraryGuid,
            ll.LAST_UPDATED as LastUpdated
        from libtolibrelation ll
            join library l on l.library_id = ll.relatedlibrary_id
            join library l2 on l2.library_id = ll.library_id
        {whereClause.clause}
        "; 
        return (query, whereClause.parameters);
    }
}
