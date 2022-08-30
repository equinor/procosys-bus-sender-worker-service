namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class PipeTestQuery
{
    public static string GetQuery(long? tagCheckId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(tagCheckId, plant, "co", "calloff_id");
        whereClause += "and co.calloffno is not null";

        return @$"select
        '{{""Plant"" : ""' || co.projectschema ||
        '"", ""CallOffId"" : ""' || co.calloff_id ||
        '"", ""CallOffNo"" : ""' || regexp_replace(co.calloffno, '([""\])', '\\\1') ||
        '"", ""PackageId"" : ""' || co.package_id ||
        '"", ""PurchaseOrderNo"" : ""' || regexp_replace(po.packageno, '([""\])', '\\\1') ||

        '"", ""CreatedAt"" : ""' || TO_CHAR(co.createdat, 'yyyy-mm-dd hh24:mi:ss') ||
        '""}}' as message
        from pipetest pt
            join purchaseorder po on po.package_id = co.package_id       
            left join responsible r on r.responsible_id = co.responsible_id
            left join library contractor on contractor.library_id = co.contractor_id
            left join library supplier on supplier.library_id = co.supplier_id
        {whereClause}";
    }

}
