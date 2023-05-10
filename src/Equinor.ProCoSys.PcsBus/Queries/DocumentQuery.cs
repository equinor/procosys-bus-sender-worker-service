namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class DocumentQuery
{
    public static string GetQuery(long? documentId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(documentId, plant, "d", "document_id");

        return @$"select
            d.projectschema as Plant,
            d.procosys_guid as ProCoSysGuid,
            p.name as ProjectName,
            d.document_id as DocumentId,
            d.documentno as DocumentNo,
            d.title as Title,
            apc.code as AcceptanceCode,
            arc.code as Archive,
            acc.code as AccessCode,
            com.code as Complex,
            DT.code as DocumentType,
            dp.code as DisciplineId,
            dc.code as DocumentCategory,
            ho.code as HandoverStatus,
            RT.code as RegisterType,
            d.revisionno as RevisionNo,
            rev.code as RevisionStatus,
            res.code as ResponsibleContractor,
            d.last_Updated as LastUpdated,
            d.revisiondate as RevisionDate
        from document d
            left join project p on p.project_id = d.project_id
            left join library RT on RT.library_id = d.register_id
            left join library DT on DT.library_id = d.documenttype_id
            left join library apc on apc.library_id = d.acceptancecode_id
            left join library dc on dc.library_id = d.documentcategory_id
            left join library arc on arc.library_id = d.archive_id
            left join library dp on dp.library_id = d.discipline_id
            left join library rev on rev.library_id = d.revisionstatus_id
            left join library ho on ho.library_id = d.handoverstatus_id
            left join library res on res.library_id = d.responsiblecontractor_id
            left join library acc on acc.library_id = d.accesscode_id
            left join library com on com.library_id = d.complex_id
        {whereClause}";
    }
}
