using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class ProjectSchemaQuery
{
    public static string GetQuery()
    {
        var query = @$"select
            p.projectschema as Plant
        from projectschema p
        where p.isvoided = 'N'";

        return query;
    }
}
