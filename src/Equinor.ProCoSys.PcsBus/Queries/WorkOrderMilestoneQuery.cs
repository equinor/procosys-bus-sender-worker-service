using System;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderMilestoneQuery
{
    private static string CreateWhereClause(long? milestoneId, long? woId, string? plant)
    {
        var whereClause = "";
        if (milestoneId != null && woId != null && plant != null)
        {
            whereClause =
                $"where emd.projectschema = '{plant}' and emd.element_id = {woId} and emd.milestone_id = {milestoneId}";
        }
        else if (plant != null)
        {
            whereClause = $"where emd.projectschema = '{plant}'";
        }
        else if (milestoneId != null && woId != null)
        {
            whereClause = $"where emd.element_id = {woId} and emd.milestone_id = {milestoneId}";
        }
        else if (milestoneId != null || woId != null)
        {
            throw new Exception(
                "Message can not contain partial id match, need both milestone and wo id to find correct db entry");
        }

        return whereClause;
    }

    /// <summary>
    /// Call with either, plantId, wo and milestone id, or all 3. Not advised to call without either as result set could get very large
    /// Both Id columns needed for single match
    /// </summary>
    public static string GetQuery(long? woId, long? milestoneId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(milestoneId, woId, plant);

        return @$"select
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
        {whereClause}";
    }
}
