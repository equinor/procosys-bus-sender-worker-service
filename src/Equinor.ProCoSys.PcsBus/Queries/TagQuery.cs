using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class TagQuery
{
    public static (string query, DynamicParameters parameters) GetQuery(long? tagId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(tagId, plant, "t", "tag_id");

        var query = @$"select
            t.projectschema as Plant,
            t.procosys_guid as ProCoSysGuid,
            t.tagno as TagNo,
            t.description as Description,
            p.name as ProjectName,
            mcpkg.mcpkgno as McPkgNo,
            mcpkg.procosys_guid as McPkgGuid,
            commpkg.commpkgno as CommPkgNo,
            commpkg.procosys_guid as CommPkgGuid,
            t.tag_id as TagId,
            area.code as AreaCode,
            area.description as AreaDescription,
            discipline.code as DisciplineCode,
            discipline.description as DisciplineDescription,
            register.code as RegisterCode,
            installation.code as InstallationCode,
            status.code as Status,
            system.code as System,
            calloff.calloffno as CallOffNo,
            calloff.procosys_guid as CallOffGuid,
            purchaseorder.packageno as PurchaseOrderNo,
            tagfunction.tagfunctioncode as TagFunctionCode,
            e.IsVoided as IsVoided,
            ps.TITLE as PlantName,
            ec.code as EngineeringCode,
            t.mountedon_id as MountedOn,
            mt.procosys_guid as MountedOnGuid,
            t.LAST_UPDATED as LastUpdated,
            '{{' || 
            (select listagg('""'|| colName ||'"":""'|| regexp_replace(val, '([""\])', '\\\1') ||'""'
            || case when colname2 is not null then ',' || '""'|| colName2 ||'"":""'|| val2 ||'""' end
            || case when colname3 is not null then ',' || '""'|| colName3 ||'"":""'|| val3 ||'""' end,
            ',')
                WITHIN group (order by colName) as tagdetails  from (
                SELECT 
                        DECODE(F.COLUMNNAME, 'FROM_TAG_NUMBER', 'FromTagGuid', NULL) AS COLNAME2,
                        DECODE(F.COLUMNNAME, 'TO_TAG_NUMBER', 'ToTagGuid', NULL) AS COLNAME3,
                        F.COLUMNNAME AS COLNAME,
                        COALESCE(REGEXP_REPLACE(VAL.VALUESTRING, '([""\])', '\\\1'),
                                 TO_CHAR(VAL.VALUEDATE, 'YYYY-MM-DD HH24:MI:SS'),
                                 TO_CHAR(VAL.VALUENUMBER),
                                 T2.TAGNO, 
                                 LIBVAL.CODE
                                 ) AS VAL,
            t2.PROCOSYS_GUID AS VAL2,
            t2.PROCOSYS_GUID AS VAL3                                 
                FROM DEFINEELEMENTFIELD DEF
                    LEFT JOIN FIELD F ON DEF.FIELD_ID = F.FIELD_ID
                    LEFT JOIN LIBRARY UNIT ON UNIT.LIBRARY_ID = F.UNIT_ID
                    JOIN ELEMENTFIELD VAL
                        ON (VAL.FIELD_ID = DEF.FIELD_ID AND VAL.ELEMENT_ID = t.tag_id)
                    JOIN TAG t1 on t1.TAG_ID = VAL.ELEMENT_ID
                    LEFT JOIN LIBRARY LIBVAL ON (LIBVAL.LIBRARY_ID = VAL.LIBRARY_ID)
                    LEFT JOIN LIBRARY REG ON REG.LIBRARY_ID = DEF.REGISTER_ID
                    LEFT JOIN TAG t2 ON t2.TAG_ID = VAL.TAG_ID
                WHERE DEF.ELEMENTTYPE = 'TAG'
                AND (DEF.REGISTER_ID IS NULL OR DEF.REGISTER_ID = t.register_id)
                AND NOT (DEF.ISVOIDED = 'Y')
                AND F.COLUMNTYPE in ('NUMBER','DATE','STRING', 'LIBRARY','TAG')
                AND f.projectschema ='{plant}'))
                || '}}' as TagDetails
        from tag t
            join element e on e.element_id = t.tag_id
            join projectschema ps on ps.projectschema = t.projectschema
            left join mcpkg on mcpkg.mcpkg_id = t.mcpkg_id
            left join commpkg on commpkg.commpkg_id= COALESCE(mcpkg.commpkg_id,t.commpkg_id)
            left join project p on p.project_id = t.project_id
            left join tagfunction tf on tf.tagfunction_id = t.tagfunction_id
            left join library area on area.library_id = t.area_id
            left join library discipline on discipline.library_id = t.discipline_id
            left join library register on register.library_id = t.register_id
            left join library installation on installation.library_id = t.installation_id
            left join library status on status.library_id = t.status_id
            left join library system on system.library_id = t.system_id
            left join calloff  on calloff.calloff_id = t.calloff_id
            left join purchaseorder on purchaseorder.package_id = calloff.package_id
            left join tagfunction on tagfunction.tagfunction_id = t.tagfunction_id     
            left join library ec on ec.library_id = t.engineeringcode_id
            left join tag mt on t.mountedon_id = mt.tag_id
        {whereClause.clause}";
        
        return (query, whereClause.parameters);
    }
}
