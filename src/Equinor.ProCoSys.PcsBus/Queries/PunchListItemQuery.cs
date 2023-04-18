namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class PunchListItemQuery
{
    public static string GetQuery(long? punchListItemId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(punchListItemId, plant, "pl", "punchlistitem_id");

        return @$"select
            pl.projectschema as Plant,
            pl.procosys_guid as ProCoSysGuid,
            p.name as ProjectName,
            pl.LAST_UPDATED as LastUpdated,
            pl.PunchListItem_Id as PunchItemNo,
            pl.Description as Description,
            pl.tagcheck_id as ChecklistId,
            tc.procosys_guid as ChecklistGuid,
            cat.code as Category,
            raised.code as RaisedByOrg,
            cleared.code as ClearingByOrg,
            pl.duedate as DueDate,
            plsorting.code as PunchListSorting,
            pltype.code as PunchListType,
            plpri.code as PunchPriority,
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
            pl.createdat as CreatedAt
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
        {whereClause}";
    }
}
