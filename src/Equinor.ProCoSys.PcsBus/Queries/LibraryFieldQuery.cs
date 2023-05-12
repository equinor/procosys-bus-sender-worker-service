using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class LibraryFieldQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string libraryFieldGuid, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(libraryFieldGuid, plant, "lf", "procosys_guid");
        var query = $@"select   
            lf.projectschema as Plant,
            lf.procosys_guid as ProCoSysGuid,
            l.procosys_guid as LibraryGuid,
            l.librarytype as LibraryType,
            l.code as Code,
            field.columnname as ColumnName,
            field.columntype as ColumnType,
            lf.valuestring as StringValue,
            lf.valuedate as DateValue,
            lf.valuenumber as NumberValue,
            lv.procosys_guid as LibraryValueGuid,
            lf.LAST_UPDATED as LastUpdated
        from libraryfield lf
            join library l on l.library_id = lf.library_id
            join definelibraryfield dlf on dlf.id = lf.definelibraryfield_id
            join field field on field.field_id = dlf.field_id
            left join library lv on lv.library_id = lf.value_id
        {whereClause.clause}";
        return (query, whereClause.parameters);
    }
}
