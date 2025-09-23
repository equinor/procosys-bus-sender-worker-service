using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class TagDocumentQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string? proCoSysGuid, string? plant = null) 
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(proCoSysGuid, plant,"td", "procosys_guid");

        var query = $@"select
            td.projectschema as Plant,
            td.procosys_guid as ProCoSysGuid,
            td.tag_id as TagId,
            t.procosys_guid as TagGuid,
            d.document_id as DocumentId,
            d.procosys_guid as DocumentGuid,
            td.last_updated as LastUpdated
        from tagdocument td
            join tag t on t.tag_id = td.tag_id
            join document d on d.document_id = td.document_id
        {whereClause.clause}";
       
        return (query,whereClause.parameters);
    }
}
