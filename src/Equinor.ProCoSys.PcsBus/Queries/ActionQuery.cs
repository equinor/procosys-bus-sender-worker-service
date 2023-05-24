// ReSharper disable StringLiteralTypo

using System.Diagnostics.CodeAnalysis;
using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

[SuppressMessage("ReSharper", "StringLiteralTypo")]
public class ActionQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(long? actionId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(actionId, plant, "ec", "element_id");

        var query = @$"select
            ec.ProjectSchema as Plant,
            ec.PROCOSYS_GUID as ProCoSysGuid,
            ec.Id as ElementContentGuid,
            c.CommPkgNo as CommPkgNo,
            c.Procosys_guid as CommPkgGuid,
            s.SwcrNo as SwcrNo,
            s.Procosys_guid as SwcrGuid,
            d.DocumentNo as DocumentNo,
            ec.Description as Description,
            d.Procosys_guid as DocumentGuid,
            SUBSTR(
                (
                    SELECT LISTAGG(CODE,'.') WITHIN GROUP (ORDER BY LEVEL DESC)
                    FROM ELEMENTCONTENT n
                    START WITH n.ELEMENT_ID=ec.ELEMENT_ID
                    CONNECT BY PRIOR n.PARENT_ID=n.ELEMENT_ID
                ),3) as ActionNo,
            ec.Title as Title,
            ec.Comments as Comments,
            action.deadline as Deadline,
            category.code as CategoryCode,
            category.procosys_guid as CategoryGuid,
            priority.code as PriorityCode,
            priority.procosys_guid as PriorityGuid,
            requestedby.azure_oid as RequestedByOid,
            actionbyperson.azure_oid as ActionByOid,
            actionfuncrole.code as ActionByRole,
            actionfuncrole.procosys_guid as ActionByRoleGuid,
            responsibleperson.azure_oid as ResponsibleOid,
            responsiblefunrole.code as ResponsibleRole,
            responsiblefunrole.procosys_guid as ResponsibleRoleGuid,
            ec.Last_Updated as LastUpdated,
            es.SignedAt as SignedAt,
            p.Azure_Oid as SignedBy
        from ELEMENTCONTENT ec
            join LIBRARY l ON ec.LIBRARY_ID = l.LIBRARY_ID AND 'MAINACTION'= l.CODE
            join ELEMENTCONTENT root ON root.ELEMENT_ID = (
                select element_id
                from elementcontent 
                where parent_id is null
                start with element_id=ec.element_id
                connect by prior parent_id=element_id
                )
            left join ELEMENTCONTENTACTION action ON action.ELEMENT_ID = ec.ELEMENT_ID
            left join LIBRARY category on category.library_id = action.category_id
            left join LIBRARY priority on priority.library_id = action.priority_id
            left join COMMPKG c ON c.COMMPKG_ID = root.ELEMENT_ID  
            left join SWCR s ON s.SWCR_ID=root.ELEMENT_ID 
            left join DOCUMENT d ON d.DOCUMENT_ID = root.ELEMENT_ID 
            left join elementsignature es ON es.ELEMENT_ID = ec.ELEMENT_ID
            left join person p ON es.SIGNEDBY_ID = p.PERSON_ID                      
            left join person requestedby ON requestedby.PERSON_ID = action.REQUESTEDBY_ID
            left join person actionbyperson ON actionbyperson.PERSON_ID = action.ACTIONBY_PERSON_ID
            left join person responsibleperson ON responsibleperson.PERSON_ID = action.RESPONSIBLE_PERSON_ID
            left join LIBRARY responsiblefunrole ON responsiblefunrole.LIBRARY_ID = action.RESPONSIBLE_FUNCTIONALROLE_ID
            left join LIBRARY actionfuncrole ON actionfuncrole.LIBRARY_ID = ACTION.ACTIONBY_FUNCTIONALROLE_ID
        {whereClause.clause}";
        return (query, whereClause.parameters);
    }
}
