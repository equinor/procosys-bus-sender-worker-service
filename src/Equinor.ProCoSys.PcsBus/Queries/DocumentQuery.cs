using static Equinor.ProCoSys.PcsServiceBus.Queries.QueryHelper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class DocumentQuery
{
    public static string GetQuery(long? documentId, string plant= null)
    {
        DetectFaultyPlantInput(plant);

        var whereClause = CreateWhereClause(documentId, plant);

        return @$"select
       '{{""Plant"": ""' || d.projectschema ||
       '"", ""ProjectName"" : ""' || p.name ||
       '"", ""DocumentId"" : ""' || d.document_id ||
       '"", ""DocumentNo"" : ""' || regexp_replace(d.documentno, '([""\])', '\\\1') ||
       '"", ""Title"" : ""' || regexp_replace(d.title, '([""\])', '\\\1') ||
       '"", ""AcceptanceCode"" : ""' || apc.code ||
       '"", ""Archive"" : ""' || arc.code ||
       '"", ""AccessCode"" : ""' || acc.code ||
       '"", ""Complex"" : ""' || com.code ||
       '"", ""DocumentType"" : ""' || DT.code ||
       '"", ""DisciplineId"" : ""' || dp.code ||
       '"", ""DocumentCategory"" : ""' || dc.code ||
       '"", ""HandoverStatus"" : ""' || ho.code ||
       '"", ""RegisterType"" : ""' || RT.code ||
       '"", ""RevisionNo"" : ""' || d.revisionno ||
       '"", ""RevisionStatus"" : ""' || rev.code ||
       '"", ""ResponsibleContractor"" : ""' || res.code ||
       '"", ""LastUpdated"" : ""' || TO_CHAR(d.last_Updated, 'yyyy-mm-dd hh24:mi:ss') ||
       '"", ""RevisionDate"" : ""' || TO_CHAR(d.revisiondate, 'yyyy-mm-dd hh24:mi:ss') ||
       '""}}' as message
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

    private static string CreateWhereClause(long? documentId, string plant)
    {
        var whereClause = "";
        if (documentId != null && plant != null)
        {
            whereClause = $"where d.projectschema = '{plant}' and d.document_id = {documentId}";
        }
        else if (plant != null)
        {
            whereClause = $"where d.projectschema = '{plant}'";
        }
        else if (documentId != null)
        {
            whereClause = $"where d.document_id = {documentId}";
        }

        return whereClause;
    }
}
