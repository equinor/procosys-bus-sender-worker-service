using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class TagDocumentQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string proCoSysGuid, string? plant = null) 
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(proCoSysGuid, plant,"er", "procosys_guid");

        var query = $@"select
            er.projectschema as Plant,
            er.procosys_guid as ProCoSysGuid,
            t.tag_id as tagId,
            t.procosys_guid as TagGuid,
            d.document_id as DocumentId,
            d.documentno as DocumentNo,
            d.procosys_guid as DocumentGuid,
            er.last_updated as LastUpdated
        from elementreference er
            join tag t on t.tag_id = er.fromelement_id
            join document d on d.document_id = er.toelement_id
        {whereClause.clause}";
       
        return (query,whereClause.parameters);
    }
}
