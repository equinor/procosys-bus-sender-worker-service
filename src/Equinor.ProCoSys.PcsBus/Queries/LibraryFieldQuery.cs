namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class LibraryFieldQuery
{
    public static string GetQuery(string libraryFieldGuid, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClauseForGuid(libraryFieldGuid, plant, "lf", "procosys_guid");
        return $@"select   
         '{{""Plant"" : ""' || lf.projectschema || '"",
            ""ProCoSysGuid"" : ""' || lf.procosys_guid || '"",
            ""LibraryGuid"" : ""' || l.procosys_guid || '"",
            ""LibraryType"" : ""' ||  regexp_replace(l.librarytype, '([""\])', '\\\1') || '"",
            ""Code"" : ""' || regexp_replace(l.code, '([""\])', '\\\1') || || '"",
            ""ColumnName"" : ""' || regexp_replace(field.columnname, '([""\])', '\\\1') || '"",
            ""ColumnType"" : ""' || regexp_replace(field.columntype, '([""\])', '\\\1') || '"",
            ""StringValue"" : ""' || regexp_replace(lf.valuestring, '([""\])', '\\\1') || '"",
            ""DateValue"" : ""' || TO_CHAR(lf.valuedate, 'yyyy-mm-dd hh24:mi:ss')  || '"",
            ""NumberValue"" : ""' || lf.valuenumber  || '"",
            ""LibraryValueGuid"" : ""' || lv.procosys_guid  || '"",
            ""LastUpdated"" : ""' || TO_CHAR(lf.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss')  || '""
            }}'
        from libraryfield lf
            join library l on l.library_id = lf.library_id
            join definelibraryfield dlf on dlf.id = lf.definelibraryfield_id
            join field field on field.field_id = dlf.field_id
            left join library lv on lv.library_id = lf.value_id
        {whereClause}";
    }
}
