﻿namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class WorkOrderChecklistsQuery
{
    public static string GetQuery(long? tagCheckId, long? woId, string plant)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(tagCheckId, woId, plant);

        return @$"select
            '{{""Plant"" : ""' || wotc.projectschema ||
            '"", ""ProjectName"" : ""' || p.NAME ||
            '"", ""WoNo"" : ""' || regexp_replace(wo.wono, '([""\])', '\\\1') ||
            '"", ""ChecklistId"" : ""' || wotc.tagcheck_id ||
            '""}}'
        FROM wo_tagcheck wotc
            join element e on E.ELEMENT_ID = wotc.wo_ID
            join wo on wo.wo_id = wotc.wo_id
            join project p ON p.project_id = wo.project_id
        {whereClause}";
    }


    private static string CreateWhereClause(long? tagCheckId, long? woId, string plant)
    {
        var whereClause = "";
        if (tagCheckId != null && woId != null && plant != null)
        {
            whereClause = $"where wotc.projectschema = '{plant}' and wotc.wo_id = {woId} and wotc.tagcheck_id = {tagCheckId}";
        }
        else if (plant != null)
        {
            whereClause = $"where wotc.projectschema = '{plant}'";
        }
        else if (tagCheckId != null && woId != null)
        {
            whereClause = $"where wotc.wo_id = {woId} and wotc.tagcheck_id = {tagCheckId}";
        }

        return whereClause;
    }

}
