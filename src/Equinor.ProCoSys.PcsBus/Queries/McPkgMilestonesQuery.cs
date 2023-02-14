namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class McPkgMilestonesQuery
{
    public static string GetQuery(string? guid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClauseForGuid(guid, plant, "e", "procosys_guid");

        return @$"SELECT e.projectschema AS Plant,
            HEXTORAW(e.procosys_guid) AS ProCoSysGuid,
            ps.TITLE AS PlantName,
            p.name AS ProjectName,
            HEXTORAW(m.procosys_guid) AS McPkgGuid,
            m.mcpkgno AS McPkgNo,
            milestone.code AS Code,
            e.actualdate AS ActualDate,
            e.planneddate AS PlannedDate,
            e.forecastdate AS ForecastDate,
            e.remark AS Remark,
            cert.issent AS IsSent,
            cert.isaccepted AS IsAccepted,
            cert.isrejected AS IsRejected,
            e.last_updated AS LastUpdated
        from completionmilestonedate e
            join projectschema ps on ps.projectschema = e.projectschema
            join library milestone on milestone.library_id = e.milestone_id
            left join commpkg c on c.commpkg_id = e.element_id
            left join mcpkg m on m.mcpkg_id = e.element_id
            left join project p on p.project_id = COALESCE(c.project_id,m.project_id)
            left join V$Certificate cert on cert.certificate_id = e.certificate_id
        {whereClause}";
    }
}
