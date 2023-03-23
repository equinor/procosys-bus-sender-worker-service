namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class PipingSpoolQuery
{
    public static string GetQuery(long? pipingSpoolId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(pipingSpoolId, plant, "ps", "pipingspool_id");

        return @$"select
            ps.projectschema as Plant,
            ps.procosys_guid as ProCoSysGuid,
            p.name as ProjectName,
            ps.pipingspool_id as PipingSpoolId,
            ps.pipingrevision_id as PipingRevisionId,
            pr.procosys_guid as PipingRevisionGuid,
            pr.testrevisionno as Revision,
            m.mcpkgno as McPkgNo,
            m.procosys_guid as McPkgGuid,
            iso.documentno as ISODrawing,
            ps.spool as Spool,
            t.tagno as LineNo,
            t.procosys_guid as LineGuid,
            ps.n2_he_test as N2HeTest,
            ps.Alternativetest as AlternativeTest,
            ps.NOOFWELDSAT as AlternativeTestNoOfWelds,
            ps.installed as Installed,
            ps.tackwelded as Welded,
            ps.weldeddate as WeldedDate,
            ps.welded as PressureTested,
            ps.nde as NDE,
            ps.primed as Primed,
            ps.painted as Painted,
            ps.LAST_UPDATED as LastUpdated
        from pipingspool ps
            join pipingrevision pr on pr.pipingrevision_id = ps.pipingrevision_id
            join mcpkg m on m.mcpkg_id = pr.mcpkg_id
            join project p on p.project_id = m.project_id
            left join document iso on iso.document_id = ps.document_id
            join tag t on t.tag_id = ps.tag_id
        {whereClause}";
    }
}
