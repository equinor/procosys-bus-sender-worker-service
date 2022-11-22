namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class HeatTraceQuery
{
    public static string GetQuery(long? heatTraceId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(heatTraceId, plant, "ht", "id");

        return @$"select
        '{{""Plant"" : ""' || ht.projectschema ||
        '"", ""ProCoSysGuid"" : ""' || ht.procosys_guid ||
        '"", ""HeatTraceId"" : ""' || ht.id ||
        '"", ""CableId"" : ""' || ht.cable_id ||
        '"", ""CableGuid"" : ""' || cable.procosys_guid ||
        '"", ""CableNo"" : ""' || regexp_replace(cable.tagno, '([""\])', '\\\1') ||
        '"", ""TagId"" : ""' || ht.tag_id ||
        '"", ""TagGuid"" : ""' || t.procosys_guid ||
        '"", ""TagNo"" : ""' || regexp_replace(t.tagno, '([""\])', '\\\1') ||
        '"", ""SpoolNo"" : ""' || ht.spoolno ||
        '"", ""LastUpdated"" : ""' || TO_CHAR(ht.last_updated, 'yyyy-mm-dd hh24:mi:ss') ||
        '""}}' as message
        from htjboxcableservice ht
            join tag cable on cable.tag_id = ht.cable_id
            join tag t on t.tag_id = ht.tag_id              
        {whereClause}";
    }
}
