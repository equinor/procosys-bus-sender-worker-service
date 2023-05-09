namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class CommPkgOperationQuery
{
    public static string GetQuery(long? commPkId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(commPkId, plant, "co","commpkg_id");

        return $@"select 
        co.projectschema as Plant,
        c.procosys_guid as ProCoSysGuid,
        p.NAME as ProjectName,
        c.commpkgno as CommPkgNo,
        c.procosys_guid as CommPkgGuid,
        co.inoperation as InOperation,
        co.readyforproduction  as ReadyForProduction,
        co.maintenanceprog as MaintenanceProgram,
        co.yellowline as YellowLine,
        co.blueline as BlueLine,
        co.yellowlinestatus AS YellowLineStatus,
        co.bluelinestatus AS BlueLineStatus,
        co.temporaryoperation_est  as TemporaryOperationEst,
        co.pmroutine as PmRoutine,
        co.commissioningresp  as CommissioningResp,
        co.valveblindinglist  as ValveBlindingList,
        co.LAST_UPDATED AS LastUpdated
    from commpkg_operation co
        join CommPkg c ON c.COMMPKG_ID = co.COMMPKG_ID
        join Project p ON c.Project_Id = p.Project_Id
    {whereClause}";
    }
}
