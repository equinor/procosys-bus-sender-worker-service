namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class SwcrAttachmentQuery
{
    public static string GetQuery(string swcrAttachmentGuid, string? plant= null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClauseForGuid(swcrAttachmentGuid, plant, "a", "procosys_guid");

        return @$"select
           '{{""Plant"" : ""' || a.ProjectSchema ||
           '"", ""ProCoSysGuid"" : ""' || a.PROCOSYS_GUID ||
           '"", ""SwcrGuid"" : ""' || s.PROCOSYS_GUID ||
           '"", ""Title"" : ""' || regexp_replace(a.TITLE, '([""\])', '\\\1') ||
           '"", ""ClassificationCode"" : ""' || a.CLASSIFICATIONCODE ||
           '"", ""URI"" : ""' || a.URI ||
           '"", ""FileName"" : ""' || regexp_replace(a.NAME, '([""\])', '\\\1') ||
           '"", ""LastUpdated"" : ""' || TO_CHAR(a.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss') ||
           '""}}' as message
            FROM ATTACHMENT a
            INNER JOIN ATTACHMENTLINK al ON a.ID = al.ATTACHMENT_ID
            INNER JOIN SWCR s ON al.ELEMENT_ID = s.SWCR_ID
            {whereClause}";
    }
}
