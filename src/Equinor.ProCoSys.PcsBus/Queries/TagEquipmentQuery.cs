namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class TagEquipmentQuery
{
    public static string GetQuery(string tagEquipmentTypeGuid, string plant= null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClauseForGuid(tagEquipmentTypeGuid, plant, "te", "procosys_guid");

        return @$"select
           '{{""Plant"" : ""' || te.ProjectSchema ||
           '"", ""ProCoSysGuid"" : ""' || te.PROCOSYS_GUID ||
           '"", ""ManufacturerName"" : ""' || regexp_replace(te.MANUFACTURER_NAME, '([""\])', '\\\1') ||
           '"", ""ModelNo"" : ""' || regexp_replace(te.MODEL_NO, '([""\])', '\\\1') ||
           '"", ""VariantNo"" : ""' || regexp_replace(te.VARIANT_NO, '([""\])', '\\\1') ||
           '"", ""EqHubId"" : ""' || regexp_replace(te.EQHUB_ID, '([""\])', '\\\1') ||
           '"", ""SemiId"" : ""' || regexp_replace(te.SEMI_ID, '([""\])', '\\\1') ||
           '"", ""ModelName"" : ""' || regexp_replace(te.MODELNAME, '([""\])', '\\\1') ||
           '"", ""ModelSubName"" : ""' || regexp_replace(te.MODELSUBNAME, '([""\])', '\\\1') ||
           '"", ""ModelSubSubName"" : ""' || regexp_replace(te.MODELSUBSUBNAME, '([""\])', '\\\1') ||
           '"", ""TagGuid"" : ""' || t.PROCOSYS_GUID ||
           '"", ""TagNo"" : ""' || regexp_replace(t.TAGNO, '([""\])', '\\\1') ||
           '"", ""ProjectName"" : ""' || regexp_replace(p.NAME, '([""\])', '\\\1') || 
           '"", ""LastUpdated"" : ""' || TO_CHAR(te.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss') ||
           '""}}' as message
            FROM TAGEQUIPMENT te
            LEFT JOIN TAG t ON te.TAG_ID = t.TAG_ID
            LEFT JOIN PROJECT p ON t.PROJECT_ID = p.PROJECT_ID
            {whereClause}";
    }
}
