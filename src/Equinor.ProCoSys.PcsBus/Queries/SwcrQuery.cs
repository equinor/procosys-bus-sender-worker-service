using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class SwcrQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? swcrId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(swcrId, plant, "sw", "swcr_id");

        var query = @$"select 
            sw.projectschema AS Plant,
            sw.procosys_guid AS ProCoSysGuid,
            p.NAME AS ProjectName,
            sw.SWCRNO AS SwcrNo,
            sw.SWCR_ID AS SwcrId,
            c.procosys_guid AS CommPkgGuid,
            c.COMMPKGNO AS CommPkgNo,
            sw.problemdescription AS Description,
            sw.modificationdescription AS Modification,
            pri.code AS Priority,
            sys.code AS System,
            cs.code AS ControlSystem,
            con.code AS Contract,
            sup.code AS Supplier,
            n.NODENO AS Node,
            STATUSFORSWCR(sw.swcr_id) AS Status,
            e.createdat AS CreatedAt,
            e.IsVoided AS IsVoided,
            sw.LAST_UPDATED AS LastUpdated,
            sw.plannedfinishdate AS DueDate,
            sw.ESTIMATEDMHRS AS EstimatedManHours,
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
        {whereClause.clause}";
        return (query, whereClause.parameters);
    }
}
