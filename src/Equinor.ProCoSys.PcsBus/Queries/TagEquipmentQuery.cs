using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class TagEquipmentQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string tagEquipmentTypeGuid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(tagEquipmentTypeGuid, plant, "te", "procosys_guid");

        var query = @$"select
            te.procosys_guid AS ProCoSysGuid,
            te.ProjectSchema AS Plant,
            te.MANUFACTURER_NAME AS ManufacturerName,
            te.MODEL_NO AS ModelNo,
            te.VARIANT_NO AS VariantNo,
            te.EQHUB_ID AS EqHubId,
            te.SEMI_ID AS SemiId,
            te.MODELNAME AS ModelName,
            te.MODELSUBNAME AS ModelSubName,
            te.MODELSUBSUBNAME AS ModelSubSubName,
            t.PROCOSYS_GUID AS TagGuid,
            t.TAGNO AS TagNo,
            p.NAME AS ProjectName,
            te.LAST_UPDATED AS LastUpdated
        from TAGEQUIPMENT te
            left join TAG t ON te.TAG_ID = t.TAG_ID
            left join PROJECT p ON t.PROJECT_ID = p.PROJECT_ID
        {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }
}
