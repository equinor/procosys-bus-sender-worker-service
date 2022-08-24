﻿namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderCutoffQuery
{
    /// <summary>
    /// Call with either workOrderId and cutoffweek, plantId  or all 3. Not advised to call without either as result set could get very large
    /// </summary>
    public static string GetQuery(long? woId, string cutoffWeek, string plant = null, string month = null)
    {
        DetectFaultyPlantInput(plant);

        var whereClause = CreateWhereClause(woId, plant, "wc", "wo_id");

        if (cutoffWeek != null)
        {
            whereClause += $"and cutoffweek = {cutoffWeek}";
        }
        else if (month != null)
        {
            whereClause += $"and TO_CHAR(wc.CUTOFFDATE, 'YYYY-MM-DD') like '%-{month}-%'";
        }

        return @$"select
            '{{""Plant"" : ""' || wc.projectschema ||
            '"", ""PlantName"" : ""' || regexp_replace(ps.TITLE, '([""\])', '\\\1') ||   
            '"", ""ProjectName"" : ""' || p.NAME ||
            '"", ""WoNo"" : ""' || regexp_replace(wo.wono, '([""\])', '\\\1') ||
            '"", ""JobStatusCode"" : ""' || regexp_replace(jsc.CODE, '([""\])', '\\\1') ||
            '"", ""MaterialStatusCode"" : ""' || regexp_replace(msc.CODE, '([""\])', '\\\1') ||
            '"", ""DisciplineCode"" : ""' || regexp_replace(dc.CODE, '([""\])', '\\\1') ||
            '"", ""CategoryCode"" : ""' || regexp_replace(cat.CODE, '([""\])', '\\\1') ||
            '"", ""MilestoneCode"" : ""' || regexp_replace(milestone.CODE, '([""\])', '\\\1') ||
            '"", ""SubMilestoneCode"" : ""' || regexp_replace(submilestone.CODE, '([""\])', '\\\1') ||
            '"", ""HoldByCode"" : ""' ||  regexp_replace(hbc.CODE, '([""\])', '\\\1') ||
            '"", ""PlanActivityCode"" : ""' ||  regexp_replace(pa.CODE, '([""\])', '\\\1') ||
            '"", ""ResponsibleCode"" : ""' || regexp_replace(r.CODE, '([""\])', '\\\1')  ||                                    
            '"", ""LastUpdated"" : ""' || TO_CHAR(wc.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss') ||
            '"", ""CutoffWeek"" : ""' || wc.CUTOFFWEEK ||
            '"", ""CutoffDate"" : ""' || TO_CHAR(wc.CUTOFFDATE, 'yyyy-mm-dd hh24:mi:ss') ||
            '"", ""PlannedStartAtDate"" : ""' || TO_CHAR(wc.WOPLANNEDSTARTUPDATE, 'yyyy-mm-dd hh24:mi:ss')  ||
            '"", ""PlannedFinishedAtDate"" : ""' || TO_CHAR(wc.WOPLANNEDCOMPLETIONDATE, 'yyyy-mm-dd hh24:mi:ss')  ||
            '"", ""ExpendedManHours"" : ""' || wc.EXPENDED_MHRS ||
            '"", ""ManhoursEarned"" : ""' || wc.EARNED_MHRS ||
            '"", ""EstimatedHours"" : ""' || wc.ESTIMATED_MHRS ||
            '"", ""ManhoursExpendedLastWeek"" : ""' ||wc.EXPENDED_LW ||
            '"", ""ManhoursEarnedLastWeek"" : ""' || wc.EARNED_LW  ||
            '"", ""ProjectProgress"" : ""' || wc.PROJECTPROGRESS ||
            '""}}' as message
        from wo_cutoff wc
            join wo wo on wo.wo_id = wc.wo_id
            join projectschema ps ON ps.projectschema = wc.projectschema
            join project p ON p.project_id = wc.project_id and p.isvoided = 'N' and p.isclosed = 'N'
            left join library milestone ON milestone.library_id = wc.womilestone_id
            left join library submilestone on submilestone.library_id = wc.wosubmilestone_id
            left join library cat ON cat.library_id = wc.category_id
            left join library msc ON msc.library_id = wc.materialstatus_id
            left join library dc ON dc.library_id = wc.discipline_id
            left join library hbc ON hbc.library_id = wc.holdby_id
            left join library pa ON pa.library_id = wc.planactivity_id
            left join library r ON r.library_id = wc.WORESPONSIBLE_id
            left join library jsc ON jsc.library_id = wc.jobstatus_id
            left join library area ON area.library_id = wc.area_id
        {whereClause}";
    }
}
