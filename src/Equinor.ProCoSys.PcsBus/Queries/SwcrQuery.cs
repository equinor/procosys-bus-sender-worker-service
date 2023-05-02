﻿namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class SwcrQuery
{
    public static string GetQuery(long? swcrId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(swcrId, plant, "sw", "swcr_id");

        return @$"select
            '{{""Plant"" : ""' || sw.projectschema ||
            '"", ""ProCoSysGuid"" : ""' || sw.procosys_guid ||
            '"", ""ProjectName"" : ""' || p.NAME ||
            '"", ""SwcrNo"" : ""' || sw.SWCRNO ||
            '"", ""SwcrId"" : ""' || sw.SWCR_ID ||
            '"", ""CommPkgGuid"" : ""' || c.procosys_guid ||
            '"", ""CommPkgNo"" : ""' || c.COMMPKGNO ||
            '"", ""Description"" : ""' || regexp_replace(sw.problemdescription, '([""\])', '\\\1') ||
            '"", ""Modification"" : ""' || regexp_replace(sw.modificationdescription, '([""\])', '\\\1') ||
            '"", ""Priority"" : ""' || pri.code ||
            '"", ""System"" : ""' || sys.code ||
            '"", ""ControlSystem"" : ""' || cs.code ||
            '"", ""Contract"" : ""' || con.code ||
            '"", ""Supplier"" : ""' || sup.code ||
            '"", ""Node"" : ""' ||  regexp_replace(n.NODENO, '([""\])', '\\\1') ||
            '"", ""Status"" : ""' ||  STATUSFORSWCR(sw.swcr_id) || 
            '"", ""CreatedAt"" : ""' || TO_CHAR(e.createdat, 'yyyy-mm-dd hh24:mi:ss')  ||
            '"", ""IsVoided"" : ""' || decode(e.IsVoided,'Y', 'true', 'N', 'false')  ||
            '"", ""LastUpdated"" : ""' || TO_CHAR(sw.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss')  ||
            '"", ""DueDate"" : ""' || TO_CHAR(sw.plannedfinishdate, 'yyyy-mm-dd hh24:mi:ss')  ||
            '"", ""EstimatedManHours"" : ""' || sw.estimatedmhrs  ||
            '""}}' as message
        from swcr sw
            join element e on  E.ELEMENT_ID = sw.swcr_ID
            join projectschema ps ON ps.projectschema = sw.projectschema
            left join project p ON p.project_id = sw.project_id
            left join commpkg c ON c.commpkg_id = sw.commpkg_id
            left join library pri ON pri.library_id = sw.priority_id
            left join library sys On sys.library_id = sw.processsystem_id
            left join library con ON con.library_id = sw.contract_id
            left join library cs ON cs.library_id = sw.controlsystem_id
            left join library sup ON sup.library_id = sw.supplier_id
            left join library act On act.library_id = sw.action_id 
            left join node n ON n.node_id = sw.node_id
        {whereClause}";
    }
}
