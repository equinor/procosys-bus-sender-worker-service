namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public static class PipingRevisionQuery
{
    public static string GetQuery(long? pipeRevId,string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(pipeRevId, plant, "pr", "pipingrevision_id");

        return @$"select
            '{{""Plant"" : ""' || pr.projectschema ||
            '"", ""ProCoSysGuid"" : ""' || pr.procosys_guid ||
            '"", ""PipingRevisionId"" : ""' || pr.pipingrevision_id ||
            '"", ""Revision"" : ""' || pr.testrevisionno || 
            '"", ""McPkgNo"" : ""' || m.mcpkgno ||
            '"", ""ProjectName"" : ""' || p.name || 
            '"", ""MaxDesignPressure"" : ""' || pr.maxdesignpressure || 
            '"", ""MaxTestPressure"" : ""' || pr.maxtestpressure || 
            '"", ""Comments"" : ""' || regexp_replace(pr.comments, '([""\])', '\\\1') ||
            '"", ""TestISODocumentNo"" : ""' ||  regexp_replace(ti.documentno, '([""\])', '\\\1') ||
            '"", ""TestISORevision"" : ""' ||  regexp_replace(pr.TEST_ISO_REVISIONNO, '([""\])', '\\\1') ||
            '"", ""PurchaseOrderNo"" : ""' || po.packageno || 
            '"", ""CallOffNo"" : ""' || co.calloffno ||
            '"", ""CallOffGuid"" : ""' || co.procosys_guid ||
            '"", ""LastUpdated"" : ""' || TO_CHAR(pr.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss') || 
            '""}}' as message
        from pipingrevision pr              
            join mcpkg m on m.mcpkg_id = pr.mcpkg_id
            join project p on p.project_id=m.project_id
            left join document ti on ti.document_id = pr.document_id
            left join purchaseorder po on po.package_id = pr.package_id
            left join calloff co on co.calloff_id = pr.calloff_id
        {whereClause}";
    }
}
