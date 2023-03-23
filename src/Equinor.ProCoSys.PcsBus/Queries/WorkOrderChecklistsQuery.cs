namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderChecklistsQuery
{
    public static string GetQuery(long? tagCheckId, long? woId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(tagCheckId, woId, plant);

        return @$"select
            wotc.projectschema as Plant,
            wotc.procosys_guid as ProCoSysGuid,
            p.NAME as ProjectName,
            wotc.tagcheck_id as ChecklistId,
            tc.procosys_guid as ChecklistGuid,
            wotc.wo_id as WoId,
            wo.procosys_guid as WoGuid,
            wo.wono as WoNo,
            wotc.last_updated as LastUpdated
        FROM wo_tagcheck wotc
            join wo on wo.wo_id = wotc.wo_id
            join tagcheck tc on tc.tagcheck_id = wotc.tagcheck_id
            join project p ON p.project_id = wo.project_id
        {whereClause}";
    }

    private static string CreateWhereClause(long? tagCheckId, long? woId, string? plant)
    {
        var whereClause = "";
        if (tagCheckId != null && woId != null && plant != null)
        {
            whereClause = $"where wotc.projectschema = '{plant}' and wotc.wo_id = {woId} and wotc.tagcheck_id = {tagCheckId}";
        }
        else if (plant != null)
        {
            whereClause = $"where wotc.projectschema = '{plant}'";
        }
        else if (tagCheckId != null && woId != null)
        {
            whereClause = $"where wotc.wo_id = {woId} and wotc.tagcheck_id = {tagCheckId}";
        }

        return whereClause;
    }
}
