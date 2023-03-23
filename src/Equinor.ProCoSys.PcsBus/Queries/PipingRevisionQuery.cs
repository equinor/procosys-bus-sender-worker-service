namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public static class PipingRevisionQuery
{
    public static string GetQuery(long? pipeRevId,string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(pipeRevId, plant, "pr", "pipingrevision_id");

        return @$"select
            pr.projectschema as Plant,
            pr.procosys_guid as ProCoSysGuid,
            pr.pipingrevision_id as PipingRevisionId,
            pr.testrevisionno as Revision,
            m.mcpkgno as McPkgNo,
            m.procosys_guid as McPkgNoGuid,
            p.name as ProjectName,
            pr.maxdesignpressure as MaxDesignPressure,
            pr.maxtestpressure as MaxTestPressure,
            pr.comments as Comments,
            ti.documentno as TestISODocumentNo,
            ti.procosys_guid as TestISODocumentGuid,
            pr.TEST_ISO_REVISIONNO as TestISORevision,
            po.packageno as PurchaseOrderNo,
            co.calloffno as CallOffNo,
            co.procosys_guid as CallOffGuid,
            pr.LAST_UPDATED as LastUpdated
        from pipingrevision pr              
            join mcpkg m on m.mcpkg_id = pr.mcpkg_id
            join project p on p.project_id=m.project_id
            left join document ti on ti.document_id = pr.document_id
            left join purchaseorder po on po.package_id = pr.package_id
            left join calloff co on co.calloff_id = pr.calloff_id
        {whereClause}";
    }
}
