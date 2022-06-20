using System;
using System.Linq;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class ChecklistQuery
{
    public static string GetQuery(long? tagCheckId, string plant = null)
    {
        if (plant != null && plant.Any(char.IsWhiteSpace))
        {
            //To detect potential Sql injection 
            throw new Exception("plant should not contain spaces");
        }

        var whereClause = CreateWhereClause(tagCheckId, plant);

        return @$"select
        '{{""Plant"" : ""' || tc.projectschema || 
        '"", ""PlantName"" : ""' || regexp_replace(ps.TITLE, '([""\])', '\\\1') ||
        '"", ""ProjectName"" : ""' || p.name ||
        '"", ""TagNo"" : ""' ||  regexp_replace(t.tagno, '([""\])', '\\\1') ||
        '"", ""TagId"" : ""' ||  t.tag_id ||
        '"", ""TagRegisterId"" : ""' ||  t.register_id ||
        '"", ""ChecklistId"" : ""' || tc.tagcheck_id ||
        '"", ""TagCategory"" : ""' || reg.code ||
        '"", ""SheetNo"" : ""' || tft.sheetno ||
        '"", ""SubSheetNo"" : ""' || tft.sheetno ||
        '"", ""FormularType"" : ""' || ft.formulartype ||
        '"", ""FormularGroup"" : ""' || ft.formulargroup ||
        '"", ""SystemModule"" : ""' || fg.systemmodule ||
        '"", ""FormularDiscipline"" : ""' || mccr_disc.code ||
        '"", ""Revision"" : ""' || pir.testrevisionno ||
        '"", ""PipingRevisionMcPkNo"" : ""' || prm.mcpkgno ||
        '"", ""Responsible"" : ""' || r.code ||
        '"", ""Status"" : ""' || status.code ||
        '"", ""UpdatedAt"" : ""' || TO_CHAR(tc.updatedat, 'yyyy-mm-dd hh24:mi:ss') ||
        '"", ""CreatedAt"" : ""' || TO_CHAR(tc.createdat, 'yyyy-mm-dd hh24:mi:ss') ||
        '"", ""SignedAt"" : ""' || TO_CHAR(tc.signedat, 'yyyy-mm-dd hh24:mi:ss') ||
        '"", ""VerifiedAt"" : ""' || TO_CHAR(tc.verifiedat, 'yyyy-mm-dd hh24:mi:ss') ||
        '""}}' as message
        from tagcheck tc
            join projectschema ps on ps.projectschema = tc.projectschema
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
        {whereClause}";
    }

    private static string CreateWhereClause(long? tagCheckId, string plant)
    {
        var whereClause = "";
        if (tagCheckId != null && plant != null)
        {
            whereClause = $"where tc.projectschema = '{plant}' and tc.tagcheck_id = {tagCheckId}";
        }
        else if (plant != null)
        {
            whereClause = $"where tc.projectschema = '{plant}'";
        }
        else if (tagCheckId != null)
        {
            whereClause = $"where tc.tagcheck_id = {tagCheckId}";
        }

        return whereClause;
    }
}
