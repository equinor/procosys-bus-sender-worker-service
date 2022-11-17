using System;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class MilestonesQuery
{
    public static string GetQuery(long? elementId,long? milestoneId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(milestoneId, elementId, plant);

        return @$"select
            '{{""Plant"" : ""' || e.projectschema ||
            '"", ""ProCoSysGuid"" : ""' || e.procosys_guid ||
            '"", ""PlantName"" : ""' || regexp_replace(ps.TITLE, '([""\])', '\\\1') ||
            '"", ""ProjectName"" : ""' || p.name ||
            '"", ""CommPkgGuid"" : ""' || c.procosys_guid ||
            '"", ""McPkgGuid"" : ""' || c.procosys_guid ||
            '"", ""CommPkgNo"" : ""' || c.commpkgno ||
            '"", ""McPkgNo"" : ""' || m.mcpkgno ||
            '"", ""Code"" : ""' || milestone.code || 
            '"", ""ActualDate"" : ""' || TO_CHAR(e.actualdate, 'yyyy-mm-dd hh24:mi:ss') ||
            '"", ""PlannedDate"" : ""' || TO_CHAR(e.planneddate, 'yyyy-mm-dd hh24:mi:ss') ||
            '"", ""ForecastDate"" : ""' || TO_CHAR(e.forecastdate, 'yyyy-mm-dd hh24:mi:ss') ||
            '"", ""Remark"" : ""' || regexp_replace(e.remark, '([""\])', '\\\1') ||
            '"", ""IsSent"" : ""' || decode(cert.issent,'Y', 'true', 'N', 'false') ||
            '"", ""IsAccepted"" : ""' || decode(cert.isaccepted,'Y', 'true', 'N', 'false') ||
            '"", ""IsRejected"" : ""' || decode(cert.isrejected,'Y', 'true', 'N', 'false') ||
            '"", ""LastUpdated"" : ""' || TO_CHAR(e.last_updated, 'yyyy-mm-dd hh24:mi:ss') ||
            '""}}' as message
        from completionmilestonedate e
            join projectschema ps on ps.projectschema = e.projectschema
            join library milestone on milestone.library_id = e.milestone_id
            left join commpkg c on c.commpkg_id = e.element_id
            left join mcpkg m on m.mcpkg_id = e.element_id
            left join project p on p.project_id = COALESCE(c.project_id,m.project_id)
            left join V$Certificate cert on cert.certificate_id = e.certificate_id
        {whereClause}";
    }

    private static string CreateWhereClause(long? milestoneId, long? element, string plant)
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
