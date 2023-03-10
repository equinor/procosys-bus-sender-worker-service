namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class TagQuery
{
    public static string GetQuery(long? tagId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(tagId, plant, "t", "tag_id");

        return @$"select
            '{{'||
            '""Plant"" : ""' || t.projectschema || '"",' ||
            '""ProCoSysGuid"" : ""' || t.procosys_guid ||  '"",' ||
            '""TagNo"" : ""' || regexp_replace(t.tagno, '([""\])', '\\\1') || '"",' ||
            '""Description"" : ""' || regexp_replace(t.description, '([""\])', '\\\1') || '"",'||
            '""ProjectName"" : ""' || p.name || '"",' ||
            '""McPkgNo"" : ""' || mcpkg.mcpkgno || '"",' ||
            '""McPkgGuid"" : ""' || mcpkg.procosys_guid || '"",' ||
            '""CommPkgNo"" : ""' || commpkg.commpkgno || '"",' ||
            '""CommPkgGuid"" : ""' || commpkg.procosys_guid || '"",' ||
            '""TagId"" : ""' || t.tag_id || '"",' ||
            '""AreaCode"" : ""' || area.code || '"",' ||
            '""AreaDescription"" : ""' || regexp_replace(area.description, '([""\])', '\\\1') || '"",' ||
            '""DisciplineCode"" : ""' || discipline.code || '"",' ||
            '""DisciplineDescription"" : ""' || regexp_replace(discipline.description, '([""\])', '\\\1') || '"",' ||
            '""RegisterCode"" : ""' || register.code || '"",' ||
            '""InstallationCode"" : ""' || installation.code || '"",' ||
            '""Status"" : ""' || status.code || '"",' ||
            '""System"" : ""' || system.code || '"",' ||
            '""CallOffNo"" : ""' || calloff.calloffno || '"",' ||
            '""CallOffGuid"" : ""' || calloff.procosys_guid || '"",' ||
            '""PurchaseOrderNo"" : ""' || purchaseorder.packageno || '"",' ||
            '""TagFunctionCode"" : ""' || tagfunction.tagfunctioncode || '"",' ||
            '""IsVoided"" : ' || decode(e.IsVoided,'Y', 'true', 'N', 'false') || ',' ||
            '""PlantName"" : ""' || regexp_replace(ps.TITLE, '([""\])', '\\\1') || '"",' ||
            '""EngineeringCode"" : ""' || regexp_replace(ec.code, '([""\])', '\\\1') || '"",' ||
            '""MountedOn"" : ""' || t.mountedon_id || '"",' ||
            '""MountedOnGuid"" : ""' || mt.procosys_guid || '"",' ||
            '""LastUpdated"" : ""' || TO_CHAR(t.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss') || '"",' ||
            '""TagDetails"" : {{' ||
                ( SELECT LISTAGG('""'|| COLNAME ||'"":""'|| REGEXP_REPLACE(VAL, '([""\])', '\\\1') ||'""'
            || CASE WHEN COLNAME2 IS NOT NULL THEN ',' || '""'|| COLNAME2 ||'"":""'|| VAL2 ||'""'ELSE NULL END
            || CASE WHEN COLNAME3 IS NOT NULL THEN ',' || '""'|| COLNAME3 ||'"":""'|| VAL3 ||'""'ELSE NULL END,
            ',')
                WITHIN GROUP (ORDER BY COLNAME, COLNAME2,COLNAME3) AS TAGDETAILS  FROM (
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
            t1.PROCOSYS_GUID AS VAL2,
            t1.PROCOSYS_GUID AS VAL3                                 
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
                || '}}' ||
            '}}' as message
        from tag t
            join element e on e.element_id = t.tag_id
            join projectschema ps on ps.projectschema = t.projectschema
            left join mcpkg on mcpkg.mcpkg_id=t.mcpkg_id
            left join commpkg on commpkg.commpkg_id= COALESCE(mcpkg.commpkg_id,t.commpkg_id)
            left join project p on p.project_id=t.project_id
            left join tagfunction tf on tf.tagfunction_id = t.tagfunction_id
            left join library area on area.library_id=t.area_id
            left join library discipline on discipline.library_id=t.discipline_id
            left join library register on register.library_id=t.register_id
            left join library installation on installation.library_id = t.installation_id
            left join library status on status.library_id=t.status_id
            left join library system on system.library_id=t.system_id
            left join calloff  on calloff.calloff_id=t.calloff_id
            left join purchaseorder on purchaseorder.package_id=calloff.package_id
            left join tagfunction on tagfunction.tagfunction_id = t.tagfunction_id     
            left join library ec on ec.library_id = t.engineeringcode_id
            left join tag mt on t.mountedon_id=mt.tag_id
        {whereClause}";
    }
}
