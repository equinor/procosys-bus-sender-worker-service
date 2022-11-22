namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class TaskQuery
{
    public static string GetQuery(long? taskId, string plant= null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(taskId, plant, "ec", "element_id");
        
        return @$"select
           '{{""Plant"": ""' || ec.ProjectSchema ||
           '"", ""ProCoSysGuid"" : ""' || ec.ProCoSys_Guid ||
           '"", ""TaskParentProCoSysGuid"" : ""' || ec2.ProCoSys_Guid ||
           '"", ""PlantName"" : ""' || ps.Title ||
           '"", ""ProjectName"" : ""' || (
                SELECT project.Title 
                FROM Document 
                    LEFT JOIN Project ON Document.Project_Id=Project.Project_id 
                WHERE Document.DOCUMENT_ID = (
                    SELECT MIN (e.element_id) AS rootElement_Id 
                    FROM ELEMENTCONTENT e 
                    START WITH e.ELEMENT_ID = ec.Element_Id 
                    CONNECT BY PRIOR e.parent_Id = e.element_id 
                    GROUP BY e.parent_id 
                    HAVING parent_id IS NULL
                    )
                ) ||
           '"", ""Title"" : ""' || regexp_replace(ec.Title, '([""\])', '\\\1') ||
           '"", ""ElementContentGuid"" : ""' || ec.Id ||
           '"", ""Description"" : ""' || regexp_replace(ec.Description, '([""\])', '\\\1') ||
           '"", ""Comments"" : ""' || regexp_replace(ec.Comments, '([""\])', '\\\1') ||
           '"", ""LastUpdated"" : ""' || TO_CHAR(ec.Last_Updated, 'yyyy-mm-dd hh24:mi:ss') ||
           '"", ""SignedAt"" : ""' || TO_CHAR(es.SignedAt, 'yyyy-mm-dd hh24:mi:ss') ||
           '"", ""SignedBy"" : ""' || p.Azure_Oid ||
           '""}}' as message
        FROM ElementContent ec 
            INNER JOIN ProjectSchema ps ON ec.ProjectSchema=ps.ProjectSchema
            INNER JOIN ElementReference er on ec.Element_id=er.ToElement_id and er.Association='Task' and er.FromElement_Role='CommPkg'
            LEFT JOIN ElementContent ec2 ON ec.Parent_Id = ec2.Element_Id
            LEFT JOIN ElementSignature es ON es.Element_Id = ec.Element_Id
            LEFT JOIN Person p ON es.SignedBy_Id = p.Person_Id
        {whereClause}";
    }

}
