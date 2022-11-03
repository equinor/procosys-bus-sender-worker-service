namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class CommPkgOperationQuery
{
    public static string GetQuery(long? commPkId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(commPkId, plant, "co","commpkg_id");

        //double check if we want our own guid.

        return $@"select   '{{""Plant"" : ""' || co.projectschema || '"",
            ""ProCoSysGuid"" : ""' || c.procosys_guid || '"", 
            ""ProjectName"" : ""' || p.NAME || '"",
            ""CommPkgNo"" : ""' || c.commpkgno || '"",
            ""CommPkgGuid"" : ""' || c.procosys_guid || '"",
            ""InOperation"" : ' || decode(co.inoperation,'Y', 'true', 'N', 'false') || ',
            ""ReadyForProduction"" : ' || decode(co.readyforproduction,'Y', 'true', 'N', 'false') || ',
            ""MaintenanceProgram"" : ' || decode(co.maintenanceprog,'Y', 'true', 'N', 'false') || ',
            ""YellowLine"" : ' || decode(co.yellowline,'Y', 'true', 'N', 'false') || ',
            ""BlueLine"" : ' || decode(co.blueline,'Y', 'true', 'N', 'false') || ',
            ""YellowLineStatus"" : ""' || regexp_replace(co.yellowlinestatus, '([""\])', '\\\1') || '"",
            ""BlueLineStatus"" : ""' || regexp_replace(co.bluelinestatus, '([""\])', '\\\1') || '"",
            ""TemporaryOperationEst"" : ' || decode(co.temporaryoperation_est,'Y', 'true', 'N', 'false') || ',
            ""PmRoutine"" : ' || decode(co.pmroutine,'Y', 'true', 'N', 'false') || ',
            ""CommissioningResp"" : ' ||  decode(co.commissioningresp,'Y', 'true', 'N', 'false') || ',
            ""ValveBlindingList"" : ""' || decode(co.valveblindinglist,'Y', 'true', 'N', 'false') ||  '"", 
            ""LastUpdated"" : ""' || TO_CHAR(co.LAST_UPDATED, 'yyyy-mm-dd hh24:mi:ss')  || '""
            }}'
        from commpkg_operation co
            join commpkg c on c.commpkg_id = co.commpkg_id
            join project p ON p.project_id = c.project_id
        {whereClause}";
    }
}
