using System;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class MilestonesQuery
{
    public static string GetQuery(long? elementId,long? milestoneId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(milestoneId, elementId, plant);

        return @$"SELECT e.projectschema AS Plant,
            HEXTORAW(e.procosys_guid) AS ProCoSysGuid,
            ps.TITLE AS PlantName,
            p.name AS ProjectName,
            HEXTORAW(c.procosys_guid) AS CommPkgGuid,
            HEXTORAW(m.procosys_guid) AS McPkgGuid,
            c.commpkgno AS CommPkgNo,
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

    private static string CreateWhereClause(long? milestoneId, long? element, string? plant)
    {
        var whereClause = "";
        if (milestoneId != null && element != null && plant != null)
        {
            whereClause = $"where e.projectschema = '{plant}' and e.element_id = {element} and e.milestone_id = {milestoneId}";
        }
        else if (plant != null)
        {
            whereClause = $"where e.projectschema = '{plant}'";
        }
        else if (milestoneId != null && element != null)
        {
            whereClause = $"where e.element_id = {element} and e.milestone_id = {milestoneId}";
        }
        else if (milestoneId != null ^ element != null)
        {
            throw new Exception("Message can not contain partial id match, need both milestone and element id to find correct db entry");
        }

        return whereClause;
    }
}
