using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class SwcrAttachmentQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string? swcrAttachmentGuid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(swcrAttachmentGuid, plant, "a", "procosys_guid");

        var query = @$"select
            a.ProjectSchema as Plant,
            a.PROCOSYS_GUID as ProCoSysGuid,
            s.PROCOSYS_GUID as SwcrGuid,
            a.TITLE as Title,
            a.CLASSIFICATIONCODE as ClassificationCode,
            a.URI as Uri,
            a.NAME as FileName,
            a.LAST_UPDATED as LastUpdated
        from ATTACHMENT a
            join ATTACHMENTLINK al ON a.ID = al.ATTACHMENT_ID
            join SWCR s ON al.ELEMENT_ID = s.SWCR_ID
        {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }
}
