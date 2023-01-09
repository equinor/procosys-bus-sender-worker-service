namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class CommPkgPriorityQuery
{
    public static string GetQuery(string? libraryContentId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClauseForGuid(libraryContentId, plant, "lf", "procosys_guid");
        whereClause += " and l.librarytype = 'COMM_PRIORITY'";

        return $@"select   
         '{{""Plant"" : ""' || lf.projectschema || '"",
            ""ProCoSysGuid"" : ""' || lf.procosys_guid || '"",
            ""LibraryGuid"" : ""' || l.procosys_guid || '"",
            ""Code"" : ""' || l.code || '"",
            ""ColumnName"" : ""' || regexp_replace(field.columnname, '([""\])', '\\\1') || '"",
            ""ColumnType"" : ""' || regexp_replace(field.columntype, '([""\])', '\\\1') || '"",
            ""Value"" : ' || COALESCE(TO_CHAR(lf.valuedate, 'yyyy-mm-dd hh24:mi:ss'), regexp_replace(lf.valuestring, '([""\])', '\\\1')) || '""
            ""LastUpdated"" : ""' || TO_CHAR(lf.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss')  || '""
            }}'
        from libraryfield lf
            join library l on l.library_id = lf.library_id
            join definelibraryfield dlf on dlf.id = lf.definelibraryfield_id
            join field field on field.field_id = dlf.field_id
        {whereClause}";
    }
}
