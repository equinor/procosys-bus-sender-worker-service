namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class ActionQuery
{
    public static string GetQuery(long? actionId, string plant= null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(actionId, plant, "ec", "element_id");

        return @$"select
           '{{""Plant"": ""' || ec.ProjectSchema ||
           '"", ""ProCoSysGuid"" : ""' || ec.PROCOSYS_GUID ||
           '"", ""ActionNo"" : ""' || SUBSTR(
                    (
                        SELECT LISTAGG(CODE,'.') WITHIN GROUP (ORDER BY LEVEL DESC)
                        FROM ELEMENTCONTENT n
                        START WITH n.ELEMENT_ID=ec.ELEMENT_ID
                        CONNECT BY PRIOR n.PARENT_ID=n.ELEMENT_ID
                    ),3) ||
           '"", ""Title"" : ""' || regexp_replace(ec.Title, '([""\])', '\\\1') ||
           '"", ""ElementContentGuid"" : ""' || ec.Id ||
           '"", ""Description"" : ""' || regexp_replace(ec.Description, '([""\])', '\\\1') ||
           '"", ""CommPkgNo"" : ""' || c.CommPkgNo ||
           '"", ""CommPkgGuid"" : ""' || c.Procosys_guid ||
           '"", ""SwcrNo"" : ""' || s.SwcrNo ||
           '"", ""SwcrGuid"" : ""' || s.Procosys_guid ||
           '"", ""DocumentNo"" : ""' || regexp_replace(d.DocumentNo, '([""\])', '\\\1') ||
           '"", ""DocumentGuid"" : ""' || d.Procosys_guid ||
           '"", ""LastUpdated"" : ""' || TO_CHAR(ec.Last_Updated, 'yyyy-mm-dd hh24:mi:ss') ||
           '"", ""SignedAt"" : ""' || TO_CHAR(es.SignedAt, 'yyyy-mm-dd hh24:mi:ss') ||
           '"", ""SignedBy"" : ""' || p.Azure_Oid ||
           '""}}' as message
            FROM ELEMENTCONTENT ec
            INNER JOIN LIBRARY l ON ec.LIBRARY_ID=l.LIBRARY_ID AND 'MAINACTION'=l.CODE
            INNER JOIN ELEMENTCONTENT root ON root.ELEMENT_ID = (
                select element_id
                from elementcontent 
                where parent_id is null
                start with element_id=ec.element_id
                connect by prior parent_id=element_id
                )
            LEFT JOIN COMMPKG c ON c.COMMPKG_ID=root.ELEMENT_ID  
            LEFT JOIN SWCR s ON s.SWCR_ID=root.ELEMENT_ID 
            LEFT JOIN DOCUMENT d ON d.DOCUMENT_ID=root.ELEMENT_ID 
            LEFT JOIN elementsignature es ON es.ELEMENT_ID = ec.ELEMENT_ID
            LEFT JOIN PERSON p ON es.SIGNEDBY_ID=p.PERSON_ID                                             
            {whereClause}";
    }
}
