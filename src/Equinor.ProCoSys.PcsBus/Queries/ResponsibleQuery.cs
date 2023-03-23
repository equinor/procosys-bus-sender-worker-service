namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class ResponsibleQuery
{
    public static string GetQuery(long? responsibleId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(responsibleId, plant, "r", "responsible_id");

        return @$"select
            r.projectschema as Plant,
            r.procosys_guid as ProCoSysGuid,
            r.responsible_id as ResponsibleId,
            r.code as Code,
            r.responsiblegroup as ResponsibleGroup,
            r.description as Description,
            r.isVoided as IsVoided,
            r.LAST_UPDATED as LastUpdated
        from responsible r
        {whereClause}";
    }
}
