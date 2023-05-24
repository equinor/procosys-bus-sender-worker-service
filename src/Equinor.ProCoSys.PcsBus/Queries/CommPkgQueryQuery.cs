using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

//Duplicate of QueryCommPkgQuery, we're keeping this(commpkgQueryQuery) long term, as even if its bad naming, its less bad that the other one.
//QueryCommPkgQuery is for now in use in other project(fam-feeder-function), only remove if you are certain.
public class CommPkgQueryQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? commPkgId, long? documentId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(commPkgId, documentId, plant);
        const string Query = "QUERY";

        var query = $@"select
            er.projectschema as Plant,
            er.procosys_guid as ProCoSysGuid,
            p.name as ProjectName,
            c.commpkg_id as CommPkgId,
            c.procosys_guid as CommPkgGuid,
            c.commpkgno as CommPkgNo,
            d.document_id as DocumentId,
            d.documentno as QueryNo,
            d.procosys_guid as QueryGuid,
            er.last_updated as LastUpdated
        from elementreference er
            join commpkg c on c.commpkg_id = er.fromelement_id
            join project p on p.project_id = c.project_id
            join document d on d.document_id = er.toelement_id
            join library l on l.library_id = d.register_id and l.code = '{Query}'
        {whereClause.clause}";
        
        return (query,whereClause.parameters);
    }

    private static (string clause, DynamicParameters parameters) CreateWhereClause(long? commPkgId, long? documentId, string? plant)
    {
        var whereClause = "";
        var parameters = new DynamicParameters();

        if (commPkgId.HasValue && documentId.HasValue)
        {
            whereClause = "where er.FromElement_Id=:CommPkgId AND er.ToElement_Id=:DocumentId";
            parameters.Add(":CommPkgId", commPkgId);
            parameters.Add(":DocumentId", documentId);

            if (plant != null)
            {
                whereClause += " AND er.ProjectSchema=:Plant";
                parameters.Add(":Plant", plant);
            }
        }
        else if (plant != null)
        {
            whereClause = "where er.ProjectSchema=:Plant";
            parameters.Add(":Plant", plant);
        }

        return (whereClause, parameters);
    }
}
