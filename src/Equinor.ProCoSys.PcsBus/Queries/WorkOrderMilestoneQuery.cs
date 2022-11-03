using System;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderMilestoneQuery
{
    /// <summary>
    /// Call with either, plantId, wo and milestone id, or all 3. Not advised to call without either as result set could get very large
    /// Both Id columns needed for single match
    /// </summary>
    public static string GetQuery(long? woId,long? milestoneId,string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(milestoneId,woId, plant);

        return @$"select
            '{{""Plant"" : ""' || emd.projectschema ||
            '"", ""ProCoSysGuid"" : ""' || emd.procosys_guid ||
            '"", ""ProjectName"" : ""' || p.NAME ||
            '"", ""WoId"" : ""' || wo.wo_id ||
            '"", ""WoGuid"" : ""' || wo.procosys_guid ||
            '"", ""WoNo"" : ""' || regexp_replace(wo.wono, '([""\])', '\\\1') ||
            '"", ""Code"" : ""' || regexp_replace(milestone.code, '([""\])', '\\\1') || 
            '"", ""MilestoneDate"" : ""' || TO_CHAR(emd.milestonedate, 'yyyy-mm-dd hh24:mi:ss') ||
            '"", ""SignedByAzureOid"" : ""' ||  regexp_replace(p.azure_oid, '([""\])', '\\\1') ||
            '"", ""LastUpdated"" : ""' || TO_CHAR(emd.last_updated, 'yyyy-mm-dd hh24:mi:ss') ||        
            '""}}' as message
        from elementmilestonedate emd
            join wo on wo.wo_id = emd.element_id
            join project p on p.project_id = wo.project_id
            join library milestone on milestone.library_id = emd.milestone_id         
            left join person p on p.person_id = emd.signedby_id 
        {whereClause}";
    }

    private static string CreateWhereClause(long? milestoneId, long? woId, string plant)
    {
        var whereClause = "";
        if (milestoneId != null && woId != null && plant != null)
        {
            whereClause = $"where emd.projectschema = '{plant}' and emd.element_id = {woId} and emd.milestone_id = {milestoneId}";
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
            throw new Exception("Message can not contain partial id match, need both milestone and wo id to find correct db entry");
        }

        return whereClause;
    }

}
