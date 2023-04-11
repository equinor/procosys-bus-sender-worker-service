namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class TaskQuery
{
    public static string GetQuery(long? taskId, string plant= null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(taskId, plant, "ec", "element_id");

        /***
         * This is to filter out elementcontents that are in fact not Tasks connected to commPackages.
         * It is done in a where clause instead of inner join to avoid duplicates.
         */
        whereClause +=
            " and EXISTS(select 1 from ElementReference er where ec.Element_id = er.ToElement_id and er.Association= 'Task' and er.FromElement_Role = 'CommPkg')";

        return @$"select
           '{{""Plant"": ""' || ec.ProjectSchema ||
           '"", ""ProCoSysGuid"" : ""' || ec.ProCoSys_Guid ||
           '"", ""TaskParentProCoSysGuid"" : ""' || ec2.ProCoSys_Guid ||
           '"", ""PlantName"" : ""' || ps.Title ||
           '"", ""DocumentId"" : ""' || subquery.DocumentId ||
           '"", ""ProjectName"" : ""' || regexp_replace(subquery.ProjectName, '([""\])', '\\\1') ||
           '"", ""Title"" : ""' || regexp_replace(ec.Title, '([""\])', '\\\1') ||
           '"", ""TaskId"" : ""' || SUBSTR(
                  (
                      SELECT LISTAGG(CODE,'.') WITHIN GROUP (ORDER BY LEVEL DESC)
                      FROM ELEMENTCONTENT n
                      START WITH n.ELEMENT_ID=ec.PARENT_ID
                      CONNECT BY PRIOR n.PARENT_ID=n.ELEMENT_ID
                  ),3) || '-' || ec.CODE ||
           '"", ""ElementContentGuid"" : ""' || ec.Id ||
           '"", ""Description"" : ""' || regexp_replace(ec.Description, '([""\])', '\\\1') ||
           '"", ""Comments"" : ""' || regexp_replace(ec.Comments, '([""\])', '\\\1') ||
           '"", ""LastUpdated"" : ""' || TO_CHAR(ec.Last_Updated, 'yyyy-mm-dd hh24:mi:ss') ||
           '"", ""SignedAt"" : ""' || TO_CHAR(es.SignedAt, 'yyyy-mm-dd hh24:mi:ss') ||
           '"", ""SignedBy"" : ""' || p.Azure_Oid ||
           '""}}' as message
        FROM     
            (
                SELECT
                    project.Title as ProjectName,
                    document.DOCUMENT_ID as DocumentId
                FROM document
                LEFT JOIN Project ON Document.Project_Id = Project.Project_id
            ) subquery,
         ElementContent ec 
            JOIN ProjectSchema ps ON ec.ProjectSchema=ps.ProjectSchema
            LEFT JOIN ElementContent ec2 ON ec.Parent_Id = ec2.Element_Id
            LEFT JOIN ElementSignature es ON es.Element_Id = ec.Element_Id
            LEFT JOIN Person p ON es.SignedBy_Id = p.Person_Id
        {whereClause}
         and subquery.DocumentId = (
            SELECT MIN(e.element_id) AS rootElement_Id
            FROM ELEMENTCONTENT e
            START WITH e.ELEMENT_ID = ec.element_id
            CONNECT BY PRIOR e.parent_Id = e.element_id
            GROUP BY e.parent_id
            HAVING parent_id IS NULL
        )";
    }
}
