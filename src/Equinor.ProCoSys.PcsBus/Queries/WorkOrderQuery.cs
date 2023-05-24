using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderQuery
{
    /// <summary>
    ///     Call with either workOrderId, plantId or both. Not advised to call without either as result set could get very
    ///     large
    /// </summary>
    public static (string queryString, DynamicParameters parameters) GetQuery(long? workOrderId = null, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(workOrderId, plant, "w", "wo_id");

        var query = @$"select
                w.projectschema as Plant, 
                w.procosys_guid as ProCoSysGuid,
                p.NAME as ProjectName, 
                w.wono as WoNo,
                w.WO_ID as WoId,
                c.COMMPKGNO as CommPkgNo,
                c.procosys_guid as CommPkgGuid,
                w.DESCRIPTIONSHORT as Title,
                w.DESCRIPTIONLONG as Description,
                milestone.CODE as MilestoneCode,
                submilestone.CODE as SubMilestoneCode,
                milestone.description as MilestoneDescription,
                cat.CODE as CategoryCode,
                msc.CODE as MaterialStatusCode,
                hbc.CODE as HoldByCode,
                dis.CODE as DisciplineCode,
                dis.description as DisciplineDescription,
                r.CODE as ResponsibleCode,
                r.description as ResponsibleDescription,
                area.CODE as AreaCode,
                area.description as AreaDescription,
                jsc.CODE as JobStatusCode,
                w.WBS as WBS,
                w.MATERIALDESCRIPTION as MaterialComments,
                w.ASSISTANCEDESCRIPTION as ConstructionComments,
                tow.CODE as TypeOfWorkCode,
                osos.CODE as OnShoreOffShoreCode,
                woc.CODE as WoTypeCode,
                w.PROJECTPROGRESS as ProjectProgress,
                w.PROGRESS as Progress,
                NVL(w.TOTALEXPENDEDMANHOURS, w.expended_mhrs) as ExpendedManHours,
                (SELECT 
                    ROUND(SUM(quantity * multiplicator * disciplinefactor * normvalue), 1)
                 FROM wo_estimate we
                 WHERE we.wo_id = w.wo_id) as EstimatedHours,
                (SELECT 
                    ROUND(SUM(quantity * multiplicator * disciplinefactor * normvalue) * (1 - w.PROJECTPROGRESS / 100), 1)
                 FROM wo_estimate we
                 WHERE we.wo_id = w.wo_id) as RemainingHours,
                w.WOPLANNEDSTARTUPDATE as PlannedStartAtDate,
                w.WOACTUALSTARTUPDATE as ActualStartAtDate,
                w.WOPLANNEDCOMPLETIONDATE as PlannedFinishedAtDate,
                w.WOACTUALCOMPLETIONDATE as ActualFinishedAtDate,
                e.CREATEDAT as CreatedAt,
                e.isVoided as IsVoided,
                w.LAST_UPDATED as LastUpdated
            from WO w
                join project p on p.project_id = w.project_id
                join element e on E.ELEMENT_ID = w.wo_ID
                left join commpkg c on c.commpkg_id = w.commpkg_id
                left join library milestone on milestone.library_id = w.womilestone_id
                left join library submilestone on submilestone.library_id = w.wosubmilestone_id
                left join library cat on cat.library_id = w.category_id
                left join library msc on msc.library_id = w.materialstatus_id
                left join library hbc on hbc.library_id = w.holdby_id
                left join library dis on dis.library_id = w.discipline_id
                left join library r on r.library_id = w.WORESPONSIBLE_id
                left join library area on area.library_id = w.area_id
                left join library jsc on jsc.library_id = w.jobstatus_id
                left join library tow on tow.library_id = w.typeofwork_id
                left join library osos on osos.library_id = w.onshoreoffshore_id
                left join library woc on woc.library_id = w.wo_id
            {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }
}
