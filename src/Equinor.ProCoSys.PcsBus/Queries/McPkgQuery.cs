namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public static class McPkgQuery
{
    public static string GetQuery(long? mcpkgId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(mcpkgId, plant, "m", "mcpkg_id");

        return @$"select
            '{{""Plant"" : ""' || e.projectschema ||
            '"", ""ProCoSysGuid"" : ""' || m.procosys_guid ||
            '"", ""PlantName"" : ""' || regexp_replace(ps.TITLE, '([""\])', '\\\1') ||
            '"", ""ProjectName"" : ""' || p.name || 
            '"", ""McPkgNo"" : ""' || m.MCPKGNO ||
            '"", ""McPkgId"" : ""' || m.MCPKG_ID ||
            '"", ""CommPkgNo"" : ""' || c.commpkgno ||
            '"", ""CommPkgGuid"" : ""' || c.procosys_guid ||
            '"", ""Description"" : ""' || regexp_replace(m.DESCRIPTION, '([""\])', '\\\1') ||
            '"", ""Remark"" : ""' || regexp_replace(m.REMARK, '([""\])', '\\\1') ||
            '"", ""ResponsibleCode"" : ""' || regexp_replace(resp.CODE, '([""\])', '\\\1') ||
            '"", ""ResponsibleDescription"" : ""' || regexp_replace(resp.DESCRIPTION, '([""\])', '\\\1') ||
            '"", ""AreaCode"" : ""' || regexp_replace(area.CODE, '([""\])', '\\\1') ||
            '"", ""AreaDescription"" : ""' || regexp_replace(area.DESCRIPTION, '([""\])', '\\\1') ||
            '"", ""Discipline"" : ""' || regexp_replace(discipline.DESCRIPTION, '([""\])', '\\\1') ||
            '"", ""McStatus"" : ""' || mcstatus.CODE ||
            '"", ""Phase"" : ""' || phase.CODE ||
            '"", ""IsVoided"" :' || decode(e.isVoided,'Y', 'true', 'N', 'false') ||
            ', ""CreatedAt"" : ""' || TO_CHAR(e.CREATEDAT, 'yyyy-mm-dd hh24:mi:ss') ||
            '"", ""LastUpdated"" : ""' || TO_CHAR(m.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss') ||
            '""}}'  as message
        from mcpkg m
            join projectschema ps on ps.projectschema = m.projectschema
            join project p on p.project_id = m.project_id
            join commpkg c on c.commpkg_id = m.commpkg_id
            join element e on e.element_id = m.mcpkg_id
            left join library discipline on discipline.library_id = m.discipline_id
            left join library area on area.library_id = m.area_id
            left join library mcstatus on mcstatus.library_id = m.mcstatus_id
            left join library phase on phase.library_id = m.mcpkgphase_id
            left join responsible resp on resp.responsible_id = m.responsible_id
        {whereClause}";
    }
}
