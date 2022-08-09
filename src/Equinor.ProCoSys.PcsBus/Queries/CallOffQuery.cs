namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class CallOffQuery
{

    public static string GetQuery(long? tagCheckId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(tagCheckId, plant, "co", "calloff_id");

        return @$"select
        '{{""Plant"" : ""' || co.projectschema ||
        '"", ""ProjectName"" : ""' || p.name ||
        '"", ""CallOffId"" : ""' || co.calloff_id ||
        '"", ""CallOffNo"" : ""' || regexp_replace(co.calloffno, '([""\])', '\\\1') ||
        '"", ""PurchaseOrderNo"" : ""' || regexp_replace(po.packageno, '([""\])', '\\\1') ||
        '"", ""IsCompleted"" : ' || decode(co.iscompleted,'Y', 'true', 'N', 'false') ||
        ', ""UseMcScope"" : ' || decode(co.usemcscope,'Y', 'true', 'N', 'false') ||
        ', ""Description"" : ""' || regexp_replace(co.description, '([""\])', '\\\1') ||
        '"", ""ResponsibleCode"" : ""' || regexp_replace(r.code, '([""\])', '\\\1') ||
        '"", ""ContractorCode"" : ""' || regexp_replace(contractor.code, '([""\])', '\\\1') ||
        '"", ""SupplierCode"" : ""' || regexp_replace(supplier.code, '([""\])', '\\\1') ||
        '"", ""EstimatedTagCount"" : ""' || co.estimatedtagcount ||
        '"", ""FATPlanned"" : ""' || TO_CHAR(co.fat_plannedsent, 'yyyy-mm-dd hh24:mi:ss') ||
        '"", ""PackagePlannedDelivery"" : ""' || TO_CHAR(co.package_planneddelivery, 'yyyy-mm-dd hh24:mi:ss') ||
        '"", ""PackageActualDelivery"" : ""' || TO_CHAR(co.package_actualdelivery, 'yyyy-mm-dd hh24:mi:ss') ||
        '"", ""PackageClosed"" : ""' || TO_CHAR(co.package_closed, 'yyyy-mm-dd hh24:mi:ss') ||
        '"", ""McDossierSent"" : ""' || TO_CHAR(co.mcdossier_sent, 'yyyy-mm-dd hh24:mi:ss') ||
        '"", ""McDossierReceived"" : ""' || TO_CHAR(co.mcdossier_recived, 'yyyy-mm-dd hh24:mi:ss') ||
        '"", ""LastUpdated"" : ""' || TO_CHAR(co.last_updated, 'yyyy-mm-dd hh24:mi:ss') ||
        '"", ""IsVoided"" : ' || decode(co.isVoided,'Y', 'true', 'N', 'false') ||
        ', ""CreatedAt"" : ""' || TO_CHAR(co.createdat, 'yyyy-mm-dd hh24:mi:ss') ||
        '""}}' as message
        from calloff co
            join purchaseorder po on po.package_id = co.package_id
            left join responsible r on r.responsible_id = co.responsible_id
            left join library contractor on contractor.library_id = co.contractor_id
            left join library supplier on supplier.library_id = co.supplier_id
        {whereClause}";
    }
}
