namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderQuery
{
    /// <summary>
    /// Call with either workOrderId, plantId or both. Not advised to call without either as result set could get very large
    /// </summary>
    public static string GetQuery(long? workOrderId = null, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(workOrderId, plant, "w", "wo_id");

        return @$"select
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
                w.MATERIALDESCRIPTION as MaterialComments,
                w.ASSISTANCEDESCRIPTION as ConstructionComments,
                tow.CODE as TypeOfWorkCode,
                osos.CODE as OnShoreOffShoreCode,
                woc.CODE as WoTypeCode,
                w.PROJECTPROGRESS as ProjectProgress,
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
            {whereClause}";



        //    '{{""Plant"" : ""' || w.projectschema ||
        //    '"", ""ProCoSysGuid"" : ""' || w.procosys_guid ||
        //    '"", ""ProjectName"" : ""' || p.NAME || 
        //    '"", ""WoNo"" : ""' || regexp_replace(w.wono, '([""\])', '\\\1') ||
        //    '"", ""WoId"" : ""' || w.WO_ID ||
        //    '"", ""CommPkgNo"" : ""' || c.COMMPKGNO ||
        //    '"", ""CommPkgGuid"" : ""' || c.procosys_guid ||
        //    '"", ""Title"" : ""' || regexp_replace(w.DESCRIPTIONSHORT, '([""\])', '\\\1') ||
        //    '"", ""Description"" : ""' || regexp_replace(w.DESCRIPTIONLONG, '([""\])', '\\\1') ||
        //    '"", ""MilestoneCode"" : ""' || regexp_replace(milestone.CODE, '([""\])', '\\\1') ||
        //    '"", ""SubMilestoneCode"" : ""' || regexp_replace(submilestone.CODE, '([""\])', '\\\1') ||
        //    '"", ""MilestoneDescription"" : ""' || regexp_replace(milestone.description, '([""\])', '\\\1') ||   
        //    '"", ""CategoryCode"" : ""' || regexp_replace(cat.CODE, '([""\])', '\\\1') ||  
        //    '"", ""MaterialStatusCode"" : ""' || regexp_replace(msc.CODE, '([""\])', '\\\1') ||
        //    '"", ""HoldByCode"" : ""' ||  regexp_replace(hbc.CODE, '([""\])', '\\\1') ||
        //    '"", ""DisciplineCode"" : ""' ||  regexp_replace(dis.CODE, '([""\])', '\\\1') ||
        //    '"", ""DisciplineDescription"" : ""' ||  regexp_replace(dis.description, '([""\])', '\\\1') ||   
        //    '"", ""ResponsibleCode"" : ""' || regexp_replace(r.CODE, '([""\])', '\\\1')  ||                    
        //    '"", ""ResponsibleDescription"" : ""' || regexp_replace(r.description, '([""\])', '\\\1') ||                             
        //    '"", ""AreaCode"" : ""' || regexp_replace(area.CODE, '([""\])', '\\\1') ||
        //    '"", ""AreaDescription"" : ""' || regexp_replace(area.description, '([""\])', '\\\1') ||
        //    '"", ""JobStatusCode"" : ""' || regexp_replace(jsc.CODE, '([""\])', '\\\1') ||
        //    '"", ""MaterialComments"" : ""' || regexp_replace(w.MATERIALDESCRIPTION, '([""\])', '\\\1') ||
        //    '"", ""ConstructionComments"" : ""' || regexp_replace(w.ASSISTANCEDESCRIPTION, '([""\])', '\\\1') ||
        //    '"", ""TypeOfWorkCode"" : ""' || regexp_replace(tow.CODE, '([""\])', '\\\1') ||
        //    '"", ""OnShoreOffShoreCode"" : ""' || regexp_replace(osos.CODE, '([""\])', '\\\1') ||
        //    '"", ""WoTypeCode"" : ""' || regexp_replace(woc.CODE, '([""\])', '\\\1') ||
        //    '"", ""ProjectProgress"" : ""' || regexp_replace(w.PROJECTPROGRESS, '([""\])', '\\\1') ||
        //    '"", ""ExpendedManHours"" : ""' || regexp_replace(NVL(w.TOTALEXPENDEDMANHOURS, w.expended_mhrs), '([""\])', '\\\1') ||
        //    '"", ""EstimatedHours"" : ""' ||
        //          ( Select ROUND (
        //                SUM (quantity
        //                * multiplicator
        //                * disciplinefactor
        //                * normvalue), 1)
        //            FROM wo_estimate we
        //            WHERE we.wo_id = w.wo_id  )
        //          ||
        //    '"", ""RemainingHours"" : ""' ||
        //         ( SELECT  ROUND (
        //             SUM (quantity
        //                * multiplicator
        //                 * disciplinefactor
        //                 * normvalue
        //                 ) * (1 - w.PROJECTPROGRESS / 100), 1)
        //        FROM wo_estimate we
        //        WHERE we.wo_id = w.wo_id)
        //         ||
        //    '"", ""PlannedStartAtDate"" : ""' || TO_CHAR(w.WOPLANNEDSTARTUPDATE, 'yyyy-mm-dd hh24:mi:ss') ||
        //    '"", ""ActualStartAtDate"" : ""' || TO_CHAR(w.WOACTUALSTARTUPDATE, 'yyyy-mm-dd hh24:mi:ss') ||
        //    '"", ""PlannedFinishedAtDate"" : ""' || TO_CHAR(w.WOPLANNEDCOMPLETIONDATE, 'yyyy-mm-dd hh24:mi:ss') ||
        //    '"", ""ActualFinishedAtDate"" : ""' ||  TO_CHAR(w.WOACTUALCOMPLETIONDATE, 'yyyy-mm-dd hh24:mi:ss') ||
        //    '"", ""CreatedAt"" : ""' || TO_CHAR(e.CREATEDAT, 'yyyy-mm-dd hh24:mi:ss') ||
        //    '"", ""IsVoided"" : ' || decode(e.isVoided,'Y', 'true', 'N', 'false') ||
        //    ', ""LastUpdated"" : ""' || TO_CHAR(w.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss') ||        
        //    '""}}' as message
        //from WO w
        //    join project p on p.project_id = w.project_id
        //    join element e on E.ELEMENT_ID = w.wo_ID
        //    left join commpkg c on c.commpkg_id = w.commpkg_id
        //    left join library milestone on milestone.library_id = w.womilestone_id
        //    left join library submilestone on submilestone.library_id = w.wosubmilestone_id
        //    left join library cat on cat.library_id = w.category_id
        //    left join library msc on msc.library_id = w.materialstatus_id
        //    left join library hbc on hbc.library_id = w.holdby_id
        //    left join library dis on dis.library_id = w.discipline_id
        //    left join library r on r.library_id = w.WORESPONSIBLE_id
        //    left join library area on area.library_id = w.area_id
        //    left join library jsc on jsc.library_id = w.jobstatus_id
        //    left join library tow on tow.library_id = w.typeofwork_id
        //    left join library osos on osos.library_id = w.onshoreoffshore_id
        //    left join library woc on woc.library_id = w.wo_id
        //{whereClause}";
    }
}
