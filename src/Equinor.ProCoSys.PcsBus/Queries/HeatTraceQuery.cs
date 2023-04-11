// ReSharper disable StringLiteralTypo
namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public static class HeatTraceQuery
{
    public static string GetQuery(long? heatTraceId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(heatTraceId, plant, "ht", "id");

        return @$"select
            ht.projectschema as Plant,
            ht.procosys_guid as ProCoSysGuid,
            ht.id as HeatTraceId,
            ht.cable_id as CableId,
            cable.procosys_guid as CableGuid,
            cable.tagno as CableNo,
            ht.tag_id as TagId,
            t.procosys_guid as TagGuid,
            t.tagno as TagNo,
            ht.spoolno as SpoolNo,
            ht.last_updated as LastUpdated
        from htjboxcableservice ht
            join tag cable on cable.tag_id = ht.cable_id
            join tag t on t.tag_id = ht.tag_id              
        {whereClause}";
    }
}
