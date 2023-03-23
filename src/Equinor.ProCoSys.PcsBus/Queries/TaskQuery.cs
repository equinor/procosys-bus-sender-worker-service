namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class TaskQuery
{
    public static string GetQuery(long? taskId, string? plant= null)
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
           ec.ProjectSchema as Plant,
            ec.ProCoSys_Guid as ProCoSysGuid,
            ec2.ProCoSys_Guid as TaskParentProCoSysGuid,
            ps.Title as PlantName,
            (
                SELECT project.Title 
                FROM Document 
                    LEFT JOIN Project ON Document.Project_Id=Project.Project_id 
                WHERE Document.DOCUMENT_ID = (
                    SELECT MIN(e.element_id) AS rootElement_Id 
                    FROM ELEMENTCONTENT e 
                    START WITH e.ELEMENT_ID = ec.Element_Id 
                    CONNECT BY PRIOR e.parent_Id = e.element_id 
                    GROUP BY e.parent_id 
                    HAVING parent_id IS NULL
                    )
            ) as ProjectName,
            ec.Title as Title,
            SUBSTR(
                (
                    SELECT LISTAGG(CODE,'.') WITHIN GROUP (ORDER BY LEVEL DESC)
                    FROM ELEMENTCONTENT n
                    START WITH n.ELEMENT_ID = ec.PARENT_ID
                    CONNECT BY PRIOR n.PARENT_ID = n.ELEMENT_ID
                ),3) || '-' || ec.CODE as TaskId,
            ec.Id as ElementContentGuid,
            ec.Description as Description,
            ec.Comments as Comments,
            ec.Last_Updated as LastUpdated,
            es.SignedAt as SignedAt,
            p.Azure_Oid as SignedBy
        FROM ElementContent ec 
            INNER JOIN ProjectSchema ps ON ec.ProjectSchema=ps.ProjectSchema
            LEFT JOIN ElementContent ec2 ON ec.Parent_Id = ec2.Element_Id
            LEFT JOIN ElementSignature es ON es.Element_Id = ec.Element_Id
            LEFT JOIN Person p ON es.SignedBy_Id = p.Person_Id
        {whereClause}";
    }
}
