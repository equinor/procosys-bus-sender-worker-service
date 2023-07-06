using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public static class HeatTracePipeTestQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string? heatTraceId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(heatTraceId, plant, "h", "procosys_guid");

        var query = @$"select
            h.projectschema as Plant,
            h.procosys_guid as ProCoSysGuid,
            h.name as Name,
            t.procosys_guid as TagGuid,
            h.last_updated as LastUpdated
        from heattracepipetest h
            join tag t on t.tag_id = h.tag_id              
        {whereClause.clause}";

        return (query, whereClause.parameters);
    }
}
