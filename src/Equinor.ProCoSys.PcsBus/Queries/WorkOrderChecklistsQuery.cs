﻿namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderChecklistsQuery
{
    public static string GetQuery(string schema)
    {
        return @$"select
    '{{""Plant"" : ""' || wotc.projectschema ||
    '"", ""ProjectName"" : ""' || p.NAME ||
    '"", ""WoNo"" : ""' || regexp_replace(w.wono, '([""\])', '\\\1') ||
    '"", ""ChecklistId"" : ""' || wotc.tagcheck_id ||
    '""}}'
    FROM wo_tagcheck wotc
        join element e on E.ELEMENT_ID = wotc.wo_ID
        join wo on wo.wo_id = wotc.wo_id
        join project p ON p.project_id = wo.project_id
    WHERE wotc.projectschema = '{schema}'";
    }
}
