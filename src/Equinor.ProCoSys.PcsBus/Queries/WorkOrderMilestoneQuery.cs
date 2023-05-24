using System;
using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderMilestoneQuery
{
    /// <summary>
    ///     Call with either, plantId, wo and milestone id, or all 3. Not advised to call without either as result set could
    ///     get very large
    ///     Both Id columns needed for single match
    /// </summary>
    public static (string queryString, DynamicParameters parameters) GetQuery(long? woId, long? milestoneId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(milestoneId, woId, plant);

        var query = @$"select
            emd.projectschema as Plant,
            emd.procosys_guid as ProCoSysGuid,
            p.NAME as ProjectName,
            wo.wo_id as WoId,
            wo.procosys_guid as WoGuid,
            wo.wono as WoNo,
            milestone.code as Code,
            emd.milestonedate as MilestoneDate,
            p.azure_oid as SignedByAzureOid,
            emd.last_updated as LastUpdated
        from elementmilestonedate emd
            join wo on wo.wo_id = emd.element_id
            join project p on p.project_id = wo.project_id
            join library milestone on milestone.library_id = emd.milestone_id         
            left join person p on p.person_id = emd.signedby_id 
        {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }

    private static (string clause, DynamicParameters parameters) CreateWhereClause(long? milestoneId, long? woId, string? plant)
    {
        var whereClause = "";
        var parameters = new DynamicParameters();

        if (milestoneId.HasValue && woId.HasValue)
        {
            whereClause = "where emd.element_id=:WoId AND emd.milestone_id=:MilestoneId";
            parameters.Add(":WoId", woId);
            parameters.Add(":MilestoneId", milestoneId);

            if (plant != null)
            {
                whereClause += " AND emd.projectschema=:Plant";
                parameters.Add(":Plant", plant);
            }
        }
        else if (plant != null)
        {
            whereClause = "where emd.projectschema=:Plant";
            parameters.Add(":Plant", plant);
        }
        else if (milestoneId.HasValue || woId.HasValue)
        {
            throw new Exception(
                "Message can not contain partial id match, need both milestone and wo id to find correct db entry");
        }

        return (whereClause, parameters);
    }
}
