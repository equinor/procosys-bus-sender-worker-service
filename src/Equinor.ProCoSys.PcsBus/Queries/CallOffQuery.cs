using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class CallOffQuery
{
    public static (string query, DynamicParameters parameters) GetQuery(long? tagCheckId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(tagCheckId, plant, "co", "calloff_id");
        whereClause.clause += " and co.calloffno is not null";

        var query = @$"select
        co.projectschema as Plant,
        co.procosys_guid as ProCoSysGuid,
        co.calloff_id as CallOffId,
        co.calloffno as CallOffNo,
        co.package_id as PackageId,
        po.packageno as PurchaseOrderNo,
        co.iscompleted as IsCompleted,
        co.usemcscope as UseMcScope,
        co.description as Description,
        r.code as ResponsibleCode,
        contractor.code as ContractorCode,
        supplier.code as SupplierCode,
        co.estimatedtagcount as EstimatedTagCount,
        co.fat_plannedsent as FATPlanned,
        co.package_planneddelivery as PackagePlannedDelivery,
        co.package_actualdelivery as PackageActualDelivery,
        co.package_closed as PackageClosed,
        co.mcdossier_sent as McDossierSent,
        co.mcdossier_received as McDossierReceived,
        co.last_updated as LastUpdated,
        co.isVoided as IsVoided,
        co.createdat as CreatedAt
        from calloff co
            join purchaseorder po on po.package_id = co.package_id       
            left join responsible r on r.responsible_id = co.responsible_id
            left join library contractor on contractor.library_id = co.contractor_id
            left join library supplier on supplier.library_id = co.supplier_id
        {whereClause.clause}";
        
        return (query,whereClause.parameters);
    }
}
