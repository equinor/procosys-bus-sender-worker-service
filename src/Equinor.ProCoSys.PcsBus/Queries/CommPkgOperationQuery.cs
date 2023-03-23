namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class CommPkgOperationQuery
{
    public static string GetQuery(long? commPkId, string? plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(commPkId, plant, "co","commpkg_id");

        return $@"select 
        co.projectschema as Plant
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
    from ElementContent AS co
        join CommPkg c ON co.Element_Id = c.Element_Id
        join Project p ON c.Project_Id = p.Project_Id
    {whereClause}";

        //return $@"select   '{{""Plant"" : ""' || co.projectschema || '"",
        //    ""ProCoSysGuid"" : ""' || c.procosys_guid || '"", 
        //    ""ProjectName"" : ""' || p.NAME || '"",
        //    ""CommPkgNo"" : ""' || c.commpkgno || '"",
        //    ""CommPkgGuid"" : ""' || c.procosys_guid || '"",
        //    ""InOperation"" : ' || decode(co.inoperation,'Y', 'true', 'N', 'false') || ',
        //    ""ReadyForProduction"" : ' || decode(co.readyforproduction,'Y', 'true', 'N', 'false') || ',
        //    ""MaintenanceProgram"" : ' || decode(co.maintenanceprog,'Y', 'true', 'N', 'false') || ',
        //    ""YellowLine"" : ' || decode(co.yellowline,'Y', 'true', 'N', 'false') || ',
        //    ""BlueLine"" : ' || decode(co.blueline,'Y', 'true', 'N', 'false') || ',
        //    ""YellowLineStatus"" : ""' || regexp_replace(co.yellowlinestatus, '([""\])', '\\\1') || '"",
        //    ""BlueLineStatus"" : ""' || regexp_replace(co.bluelinestatus, '([""\])', '\\\1') || '"",
        //    ""TemporaryOperationEst"" : ' || decode(co.temporaryoperation_est,'Y', 'true', 'N', 'false') || ',
        //    ""PmRoutine"" : ' || decode(co.pmroutine,'Y', 'true', 'N', 'false') || ',
        //    ""CommissioningResp"" : ' ||  decode(co.commissioningresp,'Y', 'true', 'N', 'false') || ',
        //    ""ValveBlindingList"" : ""' || decode(co.valveblindinglist,'Y', 'true', 'N', 'false') ||  '"", 
        //    ""LastUpdated"" : ""' || TO_CHAR(co.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss')  || '""
        //    }}'
        //from commpkg_operation co
        //    join commpkg c on c.commpkg_id = co.commpkg_id
        //    join project p ON p.project_id = c.project_id
        //{whereClause}";
    }
}
