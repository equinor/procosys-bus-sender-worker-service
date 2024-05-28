﻿using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class PunchListItemQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? punchListItemId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(punchListItemId, plant, "pl", "punchlistitem_id");

        var query = @$"select
            pl.projectschema as Plant,
            pl.procosys_guid as ProCoSysGuid,
            p.name as ProjectName,
            p.procosys_guid as ProjectGuid,
            pl.LAST_UPDATED as LastUpdated,
            pl.PunchListItem_Id as PunchItemNo,
            pl.Description as Description,
            pl.tagcheck_id as ChecklistId,
            tc.procosys_guid as ChecklistGuid,
            cat.code as Category,
            raised.code as RaisedByOrg,
            raised.procosys_guid as RaisedByOrgGuid,
            cleared.code as ClearingByOrg,
            cleared.procosys_guid as ClearingByOrgGuid,
            pl.duedate as DueDate,
            plsorting.code as PunchListSorting,
            plsorting.procosys_guid as PunchListSortingGuid,
            pltype.code as PunchListType,
            pltype.procosys_guid as PunchListTypeGuid,
            plpri.code as PunchPriority,
            plpri.procosys_guid as PunchPriorityGuid,
            pl.estimate as Estimate,
            orgwo.wono as OriginalWoNo,
            orgwo.procosys_guid as OriginalWoGuid,
            wo.wono as WoNo,
            wo.procosys_guid as WoGuid,
            swcr.swcrno as SWCRNo,
            swcr.procosys_guid as SWCRGuid,
            doc.documentno as DocumentNo,
            doc.procosys_guid as DocumentGuid,
            pl.external_itemno as ExternalItemNo,
            pl.ismaterialrequired as MaterialRequired,
            pl.isVoided as IsVoided,
            pl.material_eta as MaterialETA,
            pl.materialno as MaterialExternalNo,
            pl.clearedat as ClearedAt,
            pl.rejectedat as RejectedAt,
            pl.verifiedat as VerifiedAt,
            pl.createdat as CreatedAt,
            pc.azure_oid as CreatedByGuid,
            pc.azure_oid as ModifiedByGuid,
            pv.azure_oid as VerifiedByGuid,
            pr.azure_oid as RejectedByGuid,
            pcl.azure_oid as ClearedByGuid,
            pa.azure_oid as ActionByGuid
        from punchlistitem pl
            join tagcheck tc on tc.tagcheck_id = pl.tagcheck_id
            join library cat on cat.library_id = pl.Status_Id
            left join Responsible r ON tc.Responsible_id = r.Responsible_Id
            left join TagFormularType tft ON tc.TagFormularType_Id = tft.TagFormularType_Id
            left join FormularType ft ON tft.FormularType_Id = ft.FormularType_Id
            left join Tag t on tft.Tag_Id = t.tag_id
            left join library reg on reg.library_id = t.register_id
            left join Project p on p.project_id=t.project_id
            left join wo on wo.wo_id = pl.wo_id
            left join library raised on raised.library_id = pl.raisedbyorg_id
            left join library cleared on cleared.library_id = pl.clearedbyorg_id
            left join library pltype on pltype.library_id = pl.punchlisttype_id
            left join library plsorting on plsorting.library_id = pl.PUNCHLISTSORTING_ID
            left join library plpri on plpri.library_id = pl.priority_id
            left join wo orgwo on orgwo.wo_id = pl.originalwo_id
            left join swcr on swcr.swcr_id = pl.swcr_id
            left join document doc on doc.document_id = pl.drawing_id
            left join Person pc on pc.person_id = pl.createdby_id
            left join Person pu on pu.person_id = pl.updatedby_id       
            left join Person pv on pv.person_id = pl.verifiedby_id
            left join Person pr on pr.person_id = pl.rejectedby_id
            left join Person pcl on pcl.person_id = pl.clearedby_id
            left join Person pa on pa.person_id = pl.actionbyperson_id
        {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }
}
