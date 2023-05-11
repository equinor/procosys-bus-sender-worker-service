namespace Equinor.ProCoSys.PcsServiceBus.Queries;

//Duplicate of QueryCommPkgQuery, we're keeping this long term, as even if its bad naming, its less bad that the other one.
//QueryCommPkgQuery is for now in use in other project(fam-feeder-function), only remove if you are certain.
public class CommPkgQueryQuery
{
    public static string GetQuery(long? commPkgId, long? documentId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(commPkgId, documentId, plant);
        const string Query = @"QUERY";

        return $@"select
            er.projectschema as Plant,
            er.procosys_guid as ProCoSysGuid,
            p.name as ProjectName,
            c.commpkg_id as CommPkgId,
            c.procosys_guid as CommPkgGuid,
            c.commpkgno as CommPkgNo,
            d.document_id as DocumentId,
            d.documentno as QueryNo,
            d.procosys_guid as QueryGuid,
            er.last_updated as Last
        from elementreference er
            join commpkg c on c.commpkg_id = er.fromelement_id
            join project p on p.project_id = c.project_id
            join document d on d.document_id = er.toelement_id
            join library l on l.library_id = d.register_id and l.code = '{Query}'
        {whereClause}";
    }

    private static string CreateWhereClause(long? commPkgId, long? documentId, string? plant)
    {
        var whereClause = "";
        if (commPkgId != null && documentId != null && plant != null)
        {
            whereClause =
                $"where er.projectschema = '{plant}' and er.fromelement_id = {commPkgId} and er.toelement_id = {documentId}";
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
