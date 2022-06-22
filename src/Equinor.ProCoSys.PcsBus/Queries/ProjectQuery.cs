namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class ProjectQuery
{
    public static string GetQuery(string schema)
    {
        return @$"select
            '{{""Plant"" : ""' || p.projectschema || 
            '"", ""ProjectName"" : ""' || p.NAME || 
            '"", ""IsClosed"" : ' || (case when p.ISVOIDED = 'Y' then 'true' else 'false' end) || 
            '"", ""Description"" : ""' || REPLACE(REPLACE(p.DESCRIPTION,'\','\\'),'""','\""') || 
            '""}}'  as message
            from project p
            where p.projectschema = '{schema}'";
    }
}
