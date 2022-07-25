namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class LoopContentQuery
{
    public static string GetQuery(long? loopTagId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(loopTagId, plant, "lt", "tag_id");

        return @$"select
        '{{""Plant"" : ""' || lt.projectschema ||
        '"", ""LoopTagId"" : ""' || lt.looptag_id ||
        '"", ""TagId"" : ""' || lt.tag_id ||
        '"", ""RegisterCode"" : ""' || register.code ||
        '"", ""LastUpdated"" : ""' || TO_CHAR(lt.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss')  ||
        '""}}'
        from looptag lt
            join tag t on t.tag_id = lt.tag_id
            left join library register on register.library_id= t.register_id
        {whereClause}";
    }
}
