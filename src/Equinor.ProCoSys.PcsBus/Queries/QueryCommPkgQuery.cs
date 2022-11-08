namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class QueryCommPkgQuery
{
    public static string GetQuery(long? commPkgId,long? documentId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(commPkgId,documentId, plant);

        return @$"select
        '{{""Plant"" : ""' || er.projectschema ||
        '"", ""ProCoSysGuid"" : ""' || er.procosys_guid ||
        '"", ""ProjectName"" : ""' || p.name ||
        '"", ""CommPkgPlaceHolderId"" : ""' || 'c.placeholderId' ||
        '"", ""CommPkgId"" : ""' || c.commpkg_id ||
        '"", ""CommPkgGuid"" : ""' || c.procosys_guid ||
        '"", ""CommPkgNo"" : ""' || c.commpkgno ||
        '"", ""DocumentId"" : ""' || d.document_id ||
        '"", ""QueryNo"" : ""' || d.documentno ||
        '"", ""QueryGuid"" : ""' || d.procosys_guid ||
        '"", ""LastUpdated"" : ""' || TO_CHAR(er.last_updated, 'yyyy-mm-dd hh24:mi:ss') ||
        '""}}' as message
        from elementreference er
            join commpkg c on c.commpkg_id = er.fromelement_id
            join project p on p.project_id = c.project_id
            join document d on d.document_id = er.toelement_id
            join library l on l.library_id = d.register_id and l.code = 'QUERY'
        {whereClause}";
    }

    private static string CreateWhereClause(long? commPkgId, long? documentId, string plant)
    {
        var whereClause = "";
        if (commPkgId != null && documentId != null && plant != null)
        {
            whereClause = $"where er.projectschema = '{plant}' and er.fromelement_id = {commPkgId} and er.toelement_id = {documentId}";
        }
        else if (plant != null)
        {
            whereClause = $"where er.projectschema = '{plant}'";
        }
        else if (commPkgId != null && documentId != null)
        {
            whereClause = $"where er.fromelement_id = {commPkgId} and er.toelement_id = {documentId}";
        }

        return whereClause;
    }
}
