namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class QueryCommPkgQuery
{
    public static string GetQuery(long? commPkgId,long? documentId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(commPkgId,documentId, plant);

        return @$"select
        '{{""Plant"" : ""' || er.projectschema ||
        '"", ""ProjectName"" : ""' || p.name ||
        '"", ""CommPkgPlaceHolderId"" : ""' || 'c.placeholderId' ||
        '"", ""CommPkgId"" : ""' || c.commpkg_id ||
        '"", ""CommPkgNo"" : ""' || c.commpkgno ||
        '"", ""DocumentId"" : ""' || d.document_id ||
        '"", ""QueryPlaceHolderId"" : ""' || 'q/d.placeholderId'  ||
        '"", ""QueryNo"" : ""' || d.documentno ||
        '"", ""LastUpdated"" : ""' || TO_CHAR(co.last_updated, 'yyyy-mm-dd hh24:mi:ss') ||
        '""}}' as message
        from elementreference er
            join commpkg c on c.commpkg_id = er.fromelement_id
            left join document d on d.document_id = er.toelement_id
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
