using Dapper;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class CommPkgQuery
{
    public static (string query, DynamicParameters parameters) GetQuery(long? commPkgId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(commPkgId, plant, "c", "commpkg_id");

        var query = @$"select
        c.projectschema as Plant,
        c.procosys_guid as ProCoSysGuid,
        ps.TITLE as PlantName,
        p.name as ProjectName,
        c.COMMPKGNO as CommPkgNo,
        c.COMMPKG_ID as CommPkgId,
        c.DESCRIPTION as Description,
        c.DESCRIPTIONOFWORK as DescriptionOfWork,
        c.REMARK as Remark,
        r.CODE as ResponsibleCode,
        r.DESCRIPTION as ResponsibleDescription,
        l.CODE  as AreaCode,
        l.DESCRIPTION as AreaDescription,
        phase.code as Phase,
        identifier.code as CommissioningIdentifier,
        e.isVoided as IsVoided,
        c.DEMOLITION as Demolition,
        e.CREATEDAT as CreatedAt,
        pri1.code as Priority1,
        pri2.code as Priority2,
        pri3.code as Priority3,
        c.PROGRESS as Progress,
        commStatus.CODE as CommPkgStatus,
        dcStatus.CODE as DCCommPkgStatus,
        c.LAST_UPDATED as LastUpdated
    from commpkg c
        join project p on p.project_id = c.project_id
        join projectschema ps on ps.projectschema = c.projectschema
        join RESPONSIBLE r on r.RESPONSIBLE_ID = c.RESPONSIBLE_ID
        join element e on e.element_id = c.commpkg_id
        left join library l on l.library_id = c.area_id
        left join library pri1 on pri1.library_id = c.COMMPRIORITY_ID
        left join library pri2 on pri2.library_id = c.COMMPRIORITY2_ID
        left join library pri3 on pri3.library_id = c.COMMPRIORITY3_ID
        left join library phase on phase.library_id = c.COMMPHASE_ID
        left join library commStatus on commStatus.library_id = c.COMMSTATUS_ID
        left join library dcStatus on dcStatus.library_id = c.DCSTATUS_ID
        left join library identifier on identifier.library_id = c.IDENTIFIER_ID
    {whereClause.clause}";
        return (query, whereClause.parameters);
    }
}
