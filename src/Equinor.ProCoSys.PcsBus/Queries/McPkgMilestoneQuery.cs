using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class McPkgMilestoneQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string? guid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(guid, plant, "e", "procosys_guid");

        var query = @$"SELECT e.projectschema AS Plant,
            e.procosys_guid AS ProCoSysGuid,
            ps.TITLE AS PlantName,
            p.name AS ProjectName,
            m.procosys_guid AS McPkgGuid,
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
            join mcpkg m on m.mcpkg_id = e.element_id
            left join project p on p.project_id = m.project_id
            left join V$Certificate cert on cert.certificate_id = e.certificate_id
        {whereClause.clause}";
        return (query, whereClause.parameters);
    }
}
