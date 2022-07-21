namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class MilestonesQuery
{
    public static string GetQuery(long? milestoneId, string schema = null)
    {
        DetectFaultyPlantInput(schema);

        var whereClause = CreateWhereClause(milestoneId, schema, "e", "milestone_id");

        return @$"select
            '{{""Plant"" : ""' || e.projectschema || 
            '"", ""PlantName"" : ""' || regexp_replace(ps.TITLE, '([""\])', '\\\1') ||
            '"", ""ProjectName"" : ""' || p.name ||  
            '"", ""CommPkgNo"" : ""' || c.commpkgno ||
            '"", ""McPkgNo"" : ""' || m.mcpkgno ||
            '"", ""Code"" : ""' || milestone.code || 
            '"", ""ActualDate"" : ""' || TO_CHAR(e.actualdate, 'yyyy-mm-dd hh24:mi:ss') ||
            '"", ""PlannedDate"" : ""' || TO_CHAR(e.planneddate, 'yyyy-mm-dd hh24:mi:ss') ||   
            '"", ""IsSent"" : ""' || decode(cert.issent,'Y', 'true', 'N', 'false') ||  
            '"", ""IsAccepted"" : ""' || decode(cert.isaccepted,'Y', 'true', 'N', 'false') ||
            '"", ""IsRejected"" : ""' || decode(cert.isrejected,'Y', 'true', 'N', 'false') || 
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
}
