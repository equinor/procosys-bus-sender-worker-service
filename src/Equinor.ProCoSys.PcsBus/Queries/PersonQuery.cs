// ReSharper disable StringLiteralTypo

using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public static class PersonQuery
{
    public static (string queryString, DynamicParameters parameters) GetQuery(string? personGuid)
    {
        var whereClause = CreateWhereClause(personGuid, null, "p", "azure_oid");

        var query = @$"select
            p.procosys_guid as ProCoSysGuid,
            p.azure_oid as AzureOid,
            p.firstname as FirstName,
            p.lastname as LastName,
            p.username as UserName,
            p.emailaddress as Email,
            p.super as SuperUser,
            p.last_updated as LastUpdated
        from procosys_auth.person p
        {whereClause.clause}";

        return (query, whereClause.parameters);
    }
}
