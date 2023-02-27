using System.Diagnostics.CodeAnalysis;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class ActionQuery
{
    public static string GetQuery(long? actionId, string plant= null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(actionId, plant, "ec", "element_id");

        return @$"select
           '{{""Plant"" : ""' || ec.ProjectSchema ||
           '"", ""ProCoSysGuid"" : ""' || ec.PROCOSYS_GUID ||
           '"", ""ElementContentGuid"" : ""' || ec.Id ||
           '"", ""CommPkgNo"" : ""' || c.CommPkgNo ||
           '"", ""CommPkgGuid"" : ""' || c.Procosys_guid ||
           '"", ""SwcrNo"" : ""' || s.SwcrNo ||
           '"", ""SwcrGuid"" : ""' || s.Procosys_guid ||
           '"", ""DocumentNo"" : ""' || regexp_replace(d.DocumentNo, '([""\])', '\\\1') ||
           '"", ""Description"" : ""' || regexp_replace(ec.Description, '([""\])', '\\\1') ||
           '"", ""DocumentGuid"" : ""' || d.Procosys_guid ||
           '"", ""ActionNo"" : ""' || SUBSTR(
                    (
                        SELECT LISTAGG(CODE,'.') WITHIN GROUP (ORDER BY LEVEL DESC)
                        FROM ELEMENTCONTENT n
                        START WITH n.ELEMENT_ID=ec.ELEMENT_ID
                        CONNECT BY PRIOR n.PARENT_ID=n.ELEMENT_ID
                    ),3) ||
           '"", ""Title"" : ""' || regexp_replace(ec.Title, '([""\])', '\\\1') ||
           '"", ""Comments"" : ""' || regexp_replace(ec.Comments, '([""\])', '\\\1') ||
           '"", ""Deadline"" : ""' || TO_CHAR(action.deadline, 'yyyy-mm-dd hh24:mi:ss') ||
           '"", ""CategoryCode"" : ""' || regexp_replace(category.code, '([""\])', '\\\1') ||
           '"", ""CategoryGuid"" : ""' || category.procosys_guid ||
           '"", ""PriorityCode"" : ""' || regexp_replace(priority.code, '([""\])', '\\\1') ||
           '"", ""PriorityGuid"" : ""' || priority.procosys_guid ||
           '"", ""RequestedByOid"" : ""' || requestedby.azure_oid ||
           '"", ""ActionByOid"" : ""' || actionbyperson.azure_oid ||
           '"", ""ActionByRole"" : ""' || regexp_replace(actionfuncrole.code, '([""\])', '\\\1') ||
           '"", ""ActionByRoleGuid"" : ""' || actionfuncrole.procosys_guid ||
           '"", ""ResponsibleOid"" : ""' || responsibleperson.azure_oid ||
           '"", ""ResponsibleRole"" : ""' ||  regexp_replace(responsiblefunrole.code, '([""\])', '\\\1') ||
           '"", ""ResponsibleRoleGuid"" : ""' || responsiblefunrole.procosys_guid  ||
           '"", ""LastUpdated"" : ""' || TO_CHAR(ec.Last_Updated, 'yyyy-mm-dd hh24:mi:ss') ||
           '"", ""SignedAt"" : ""' || TO_CHAR(es.SignedAt, 'yyyy-mm-dd hh24:mi:ss') ||
           '"", ""SignedBy"" : ""' || p.Azure_Oid ||
           '""}}' as message
            FROM ELEMENTCONTENT ec
            JOIN LIBRARY l ON ec.LIBRARY_ID = l.LIBRARY_ID AND 'MAINACTION'= l.CODE
            JOIN ELEMENTCONTENT root ON root.ELEMENT_ID = (
                select element_id
                from elementcontent 
                where parent_id is null
                start with element_id=ec.element_id
                connect by prior parent_id=element_id
                )
            LEFT JOIN ELEMENTCONTENTACTION action ON action.ELEMENT_ID = ec.ELEMENT_ID
            LEFT JOIN LIBRARY category on category.library_id = action.category_id
            LEFT JOIN LIBRARY priority on priority.library_id = action.priority_id
            LEFT JOIN COMMPKG c ON c.COMMPKG_ID = root.ELEMENT_ID  
            LEFT JOIN SWCR s ON s.SWCR_ID=root.ELEMENT_ID 
            LEFT JOIN DOCUMENT d ON d.DOCUMENT_ID = root.ELEMENT_ID 
            LEFT JOIN elementsignature es ON es.ELEMENT_ID = ec.ELEMENT_ID
            LEFT JOIN PERSON p ON es.SIGNEDBY_ID = p.PERSON_ID                      
            LEFT JOIN PERSON requestedby ON requestedby.PERSON_ID = action.REQUESTEDBY_ID
            LEFT JOIN PERSON actionbyperson ON actionbyperson.PERSON_ID = action.ACTIONBY_PERSON_ID
            LEFT JOIN PERSON responsibleperson ON responsibleperson.PERSON_ID = action.RESPONSIBLE_PERSON_ID
            LEFT JOIN LIBRARY responsiblefunrole ON responsiblefunrole.LIBRARY_ID = action.RESPONSIBLE_FUNCTIONALROLE_ID
            LEFT JOIN LIBRARY actionfuncrole ON actionfuncrole.LIBRARY_ID = ACTION.ACTIONBY_FUNCTIONALROLE_ID
            {whereClause}";
    }
}
