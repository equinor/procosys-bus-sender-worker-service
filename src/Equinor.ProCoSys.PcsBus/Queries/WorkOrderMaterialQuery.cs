namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderMaterialQuery
{
    /// <summary>
    /// Call with either workOrderId, plantId or both. Not advised to call without either as result set could get very large
    /// </summary>
    public static string GetQuery(long? woId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(woId, plant, "wm", "wo_id");

        return @$"select
            '{{""Plant"" : ""' || wm.projectschema ||
            '"", ""ProCoSysGuid"" : ""' || wm.procosys_guid ||
            '"", ""ProjectName"" : ""' || p.NAME || 
            '"", ""WoNo"" : ""' || regexp_replace(wo.wono, '([""\])', '\\\1') ||
            '"", ""WoId"" : ""' || wo.wo_id ||
            '"", ""ItemNo"" : ""' || wm.itemno || 
            '"", ""TagNo"" : ""' || regexp_replace(t.tagno, '([""\])', '\\\1') ||
            '"", ""TagId"" : ""' || wm.tag_id ||
            '"", ""TagRegisterId"" : ""' || t.register_id ||
            '"", ""StockId"" : ""' || wm.stock_id ||
            '"", ""Quantity"" : ""' || wm.quantity ||
            '"", ""UnitName"" : ""' || regexp_replace(u.name, '([""\])', '\\\1') ||
            '"", ""UnitDescription"" : ""' || regexp_replace(u.description, '([""\])', '\\\1') ||
            '"", ""AdditionalInformation"" : ""' || regexp_replace(wm.description, '([""\])', '\\\1') ||
            '"", ""RequiredDate"" : ""' || TO_CHAR(wm.requireddate, 'yyyy-mm-dd hh24:mi:ss') ||
            '"", ""EstimatedAvailableDate"" : ""' || TO_CHAR(wm.ESTIMATEDAVAILABLEDATE, 'yyyy-mm-dd hh24:mi:ss') ||
            '"", ""Available"" : ""' || decode(wm.AVAILABLE,'Y', 'true', 'N', 'false') ||
            '"", ""MaterialStatus"" : ""' ||regexp_replace(ms.code, '([""\])', '\\\1') ||       
            '"", ""StockLocation"" : ""' || regexp_replace(sl.code, '([""\])', '\\\1') ||
            '"", ""LastUpdated"" : ""' || TO_CHAR(wm.last_updated, 'yyyy-mm-dd hh24:mi:ss') ||        
            '""}}' as message
        from wo_material wm
            join wo on wo.wo_id = wm.wo_id
            join project p on p.project_id = wo.project_id       
            left join tag t on t.tag_id = wm.tag_id
            left join library ms  on ms.library_id = wm.materialstatus_id
            left join library sl on sl.library_id = wm.stocklocation_id
            left join unit u on u.unit_id = wm.unit_id 
        {whereClause}";
    }
}
