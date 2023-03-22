namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class LoopContentQuery
{
    public static string GetQuery(long? loopTagId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(loopTagId, plant, "lt", "looptag_id");

        return @$"select
            '{{""Plant"" : ""' || lt.projectschema ||
            '"", ""ProCoSysGuid"" : ""' || lt.procosys_guid ||
            '"", ""LoopTagId"" : ""' || lt.looptag_id ||
            '"", ""LoopTagGuid"" : ""' || t2.procosys_guid ||
            '"", ""TagId"" : ""' || lt.tag_id ||
            '"", ""TagGuid"" : ""' || t.procosys_guid ||
            '"", ""RegisterCode"" : ""' || register.code ||
            '"", ""LastUpdated"" : ""' || TO_CHAR(lt.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss')  ||
            '""}}'
        from looptag lt
            join tag t on t.tag_id = lt.tag_id
            join tag t2 on t2.tag_id = lt.looptag_id
            left join library register on register.library_id = t.register_id
        {whereClause}";
    }
}
