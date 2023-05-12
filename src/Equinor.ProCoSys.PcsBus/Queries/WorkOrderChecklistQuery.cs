using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderChecklistQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? tagCheckId, long? woId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(tagCheckId, woId, plant);

        var query = @$"select
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
        {whereClause.clause}";
       
        return (query, whereClause.parameters);
    }

    private static (string clause, DynamicParameters parameters) CreateWhereClause(long? tagCheckId, long? woId, string? plant)
    {
        var whereClause = "";
        var parameters = new DynamicParameters();

        if (tagCheckId.HasValue && woId.HasValue)
        {
            whereClause = "where wotc.wo_id=:WoId AND wotc.tagcheck_id=:TagCheckId";
            parameters.Add(":WoId", woId);
            parameters.Add(":TagCheckId", tagCheckId);

            if (plant != null)
            {
                whereClause += " AND wotc.projectschema=:Plant";
                parameters.Add(":Plant", plant);
            }
        }
        else if (plant != null)
        {
            whereClause = "where wotc.projectschema=:Plant";
            parameters.Add(":Plant", plant);
        }

        return (whereClause, parameters);
    }
}
