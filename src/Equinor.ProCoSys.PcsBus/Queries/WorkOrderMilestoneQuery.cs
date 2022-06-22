﻿namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderMilestoneQuery
{
    public static string GetQuery(string schema)
    {
        return @$"select
         '{{""Plant"" : ""' || emd.projectschema || 
         '"", ""ProjectName"" : ""' || p.NAME || 
         '"", ""WoNo"" : ""' || regexp_replace(w.wono, '([""\])', '\\\1') ||
         '"", ""Code"" : ""' || milestone.code || 
         '"", ""MilestoneDate"" : ""' || TO_CHAR(emd.milestonedate, 'yyyy-mm-dd hh24:mi:ss') ||
         '"", ""SignedByAzureOid"" : ""' || p.azure_oid ||
         '"", ""LastUpdated"" : ""' || TO_CHAR(emd.last_updated, 'yyyy-mm-dd hh24:mi:ss') ||        
         '""}}' as message
         from elementmilestonedate emd
            join wo on wo.wo_id = emd.element_id
            join project p on p.project_id = wo.project_id
            join library milestone on milestone.library_id = emd.milestone_id         
            left join person p on p.person_id = emd.signedby_id 
         where emd.projectschema = '{schema}'";
    }

}
