﻿namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderCutoffQuery
{
    /// <summary>
    ///     Call with either workOrderId and cutoffWeek, plantId  or all 3. Not advised to call without either as result set
    ///     could get very large
    /// </summary>
    public static string GetQuery(long? woId, string? cutoffWeek, string? plant = null, string? month = null)
    {
        DetectFaultyPlantInput(plant);

        var whereClause = CreateWhereClause(woId, plant, "wc", "wo_id");

        if (cutoffWeek != null)
        {
            whereClause += $" and wc.cutoffweek = '{cutoffWeek}'";
        }
        else if (month != null)
        {
            whereClause += $" and TO_CHAR(wc.CUTOFFDATE, 'YYYY-MM-DD') like '%-{month}-%'";
        }

        return @$"select
            wc.projectschema as Plant,
            wc.procosys_guid as ProCoSysGuid,
            ps.TITLE as PlantName,
            wo.procosys_guid as WoGuid,
            p.NAME as ProjectName,
            wo.wono as WoNo,
            jsc.CODE as JobStatusCode,
            msc.CODE as MaterialStatusCode,
            dc.CODE as DisciplineCode,
            cat.CODE as CategoryCode,
            milestone.CODE as MilestoneCode,
            submilestone.CODE as SubMilestoneCode,
            hbc.CODE as HoldByCode,
            pa.CODE as PlanActivityCode,
            r.CODE as ResponsibleCode,
            wc.LAST_UPDATED as LastUpdated,
            wc.CUTOFFWEEK as CutoffWeek,
            wc.CUTOFFDATE as CutoffDate,
            wc.WOPLANNEDSTARTUPDATE as PlannedStartAtDate,
            wc.WOPLANNEDCOMPLETIONDATE as PlannedFinishedAtDate,
            wc.EXPENDED_MHRS as ExpendedManHours,
            wc.EARNED_MHRS as ManHoursEarned,
            wc.ESTIMATED_MHRS as EstimatedHours,
            wc.EXPENDED_LW as ManHoursExpendedLastWeek,
            wc.EARNED_LW as ManHoursEarnedLastWeek,
            wc.PROJECTPROGRESS as ProjectProgress
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
