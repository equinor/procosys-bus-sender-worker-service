using Dapper;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;


[UsedImplicitly]
public class PunchPriorityLibraryRelationQuery
{
    public static (string query, DynamicParameters parameters) GetQuery(string? guid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = QueryHelper.CreateWhereClause(guid, plant, "ll", "procosys_guid");

        var query = @$"select
            ll.projectschema as Plant,
            ll.procosys_guid as ProCoSysGuid,
            l2.procosys_guid as CommPriorityGuid,
            ll.LAST_UPDATED as LastUpdated
        from libtolibrelation ll
            join library l on l.library_id = ll.relatedlibrary_id and l.code = 'PUNCH_PRIORITY'
            join library l2 on l2.library_id = ll.library_id and l2.librarytype = 'COMM_PRIORITY';
        {whereClause.clause}
        "; 
        return (query, whereClause.parameters);
    }
}
