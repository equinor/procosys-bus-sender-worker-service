namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class ChecklistQuery
{
    public static string GetQuery(long? tagCheckId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(tagCheckId, plant, "tc","tagcheck_id");

        return @$"select
            tc.projectschema as Plant, 
            hextoraw(tc.procosys_guid) as ProCoSysGuid, 
            p.name as ProjectName, 
            t.tagno as TagNo, 
            t.tag_id as TagId, 
            hextoraw(t.procosys_guid) as TagGuid, 
            t.register_id as TagRegisterId, 
            tc.tagcheck_id as ChecklistId, 
            reg.code as TagCategory, 
            tft.sheetno as SheetNo, 
            tft.subsheetno as SubSheetNo, 
            ft.formulartype as FormularType, 
            ft.formulargroup as FormularGroup, 
            phase.code as FormPhase, 
            fg.systemmodule as SystemModule, 
            mccr_disc.code as FormularDiscipline, 
            pir.testrevisionno as Revision, 
            prm.mcpkgno as PipingRevisionMcPkNo, 
            hextoraw(prm.procosys_guid) as PipingRevisionMcPkGuid, 
            r.code as Responsible, 
            status.code as Status, 
            tc.updatedat as UpdatedAt,
            tc.last_updated as LastUpdated,
            tc.createdat as CreatedAt, 
            tc.signedat as SignedAt, 
            tc.verifiedat as VerifiedAt
        from tagcheck tc
            join tagformulartype tft on tft.tagformulartype_id = tc.tagformulartype_id
            join tag t on t.tag_id = tft.tag_id
            join formulartype ft on ft.formulartype_id = tft.formulartype_id
            join formulargroup fg on fg.formulargroup = ft.formulargroup
            join library mccr_disc on mccr_disc.library_id = ft.discipline_id
            left join pipingrevision pir on pir.pipingrevision_id = tft.pipingrevision_id
            left join mcpkg prm on prm.mcpkg_id = pir.mcpkg_id
            join project p on p.project_id = t.project_id
            left join library reg on reg.library_id = t.register_id
            left join responsible r on r.responsible_id = tc.responsible_id
            left join library status on status.library_id = tc.status_id
            left join library phase on phase.library_id = tc.mcphase_id
        {whereClause}";
    }
}
