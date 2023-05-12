using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public static class McPkgQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? mcpkgId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(mcpkgId, plant, "m", "mcpkg_id");

        var query = @$"select
            e.projectschema as Plant,
            m.procosys_guid as ProCoSysGuid,
            ps.TITLE as PlantName,
            p.name as ProjectName,
            m.MCPKGNO as McPkgNo,
            m.MCPKG_ID as McPkgId,
            c.commpkgno as CommPkgNo,
            c.procosys_guid as CommPkgGuid,
            m.DESCRIPTION as Description,
            m.REMARK as Remark,
            resp.CODE as ResponsibleCode,
            resp.DESCRIPTION as ResponsibleDescription,
            area.CODE as AreaCode,
            area.DESCRIPTION as AreaDescription,
            discipline.DESCRIPTION as Discipline,
            mcstatus.CODE as McStatus,
            phase.CODE as Phase,
            e.isVoided as IsVoided,
            e.CREATEDAT as CreatedAt,
            m.LAST_UPDATED as LastUpdated
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
        {whereClause.clause}";
        return (query, whereClause.parameters);
    }
}
