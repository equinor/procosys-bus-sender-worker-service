namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class ResponsibleQuery
{
    public static string GetQuery(long? responsibleId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(responsibleId, plant, "r", "responsible_id");

        return @$"select
            '{{""Plant"" : ""' || r.projectschema ||
            '"", ""ProCoSysGuid"" : ""' || r.procosys_guid ||
            '"", ""ResponsibleId"" : ""' || r.responsible_id || 
            '"", ""Code"" : ""' || regexp_replace(r.code, '([""\])', '\\\1') ||
            '"", ""ResponsibleGroup"" : ""' || regexp_replace(r.responsiblegroup, '([""\])', '\\\1') ||
            '"", ""Description"" : ""' || regexp_replace(r.description, '([""\])', '\\\1') || 
            '"", ""IsVoided"" : ""' || decode(r.isVoided,'Y', 'true', 'N', 'false') ||
            '"", ""LastUpdated"" : ""' || TO_CHAR(r.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss') ||
            '""}}'  as message
        from responsible r
        {whereClause}";
    }
}
