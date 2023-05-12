using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderMaterialQuery
{
    /// <summary>
    ///     Call with either workOrderId, plantId or both. Not advised to call without either as result set could get very
    ///     large
    /// </summary>
    public static (string queryString, DynamicParameters parameters) GetQuery(string? guid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(guid, plant, "wm", "procosys_guid");

        var query = @$"select
            wm.projectschema as Plant,
            wm.procosys_guid as ProCoSysGuid,
            p.NAME as ProjectName,
            wo.wono as WoNo,
            wo.wo_id as WoId,
            wo.procosys_guid as WoGuid,
            wm.itemno as ItemNo,
            t.tagno as TagNo,
            wm.tag_id as TagId,
            t.procosys_guid as TagGuid,
            tl.code as TagRegisterCode,
            wm.stock_id as StockId,
            wm.quantity as Quantity,
            u.name as UnitName,
            u.description as UnitDescription,
            wm.description as AdditionalInformation,
            wm.requireddate as RequiredDate,
            wm.ESTIMATEDAVAILABLEDATE as EstimatedAvailableDate,
            wm.AVAILABLE as Available,
            ms.code as MaterialStatus,
            sl.code as StockLocation,
            wm.last_updated as LastUpdated
        from wo_material wm
            join wo on wo.wo_id = wm.wo_id
            join project p on p.project_id = wo.project_id       
            left join tag t on t.tag_id = wm.tag_id
            left join library ms  on ms.library_id = wm.materialstatus_id
            left join library sl on sl.library_id = wm.stocklocation_id
            left join unit u on u.unit_id = wm.unit_id 
            left join library tl on tl.library_id = t.register_id
        {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }
}
