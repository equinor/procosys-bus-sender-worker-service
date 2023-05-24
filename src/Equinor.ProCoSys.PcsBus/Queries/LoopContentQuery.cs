using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class LoopContentQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? loopTagId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClauseTuple = CreateWhereClause(loopTagId, plant, "lt", "looptag_id");

        var query = @$"select
            lt.projectschema as Plant,
            lt.procosys_guid as ProCoSysGuid,
            lt.looptag_id as LoopTagId,
            t2.procosys_guid as LoopTagGuid,
            lt.tag_id as TagId,
            t.procosys_guid as TagGuid,
            register.code as RegisterCode,
            lt.LAST_UPDATED as LastUpdated
        from looptag lt
            join tag t on t.tag_id = lt.tag_id
            join tag t2 on t2.tag_id = lt.looptag_id
            join library register on register.library_id = t.register_id
        {whereClauseTuple.clause}";
        return (query, whereClauseTuple.parameters);
    }
}
