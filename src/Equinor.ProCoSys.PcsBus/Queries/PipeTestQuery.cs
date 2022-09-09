using System;

namespace Equinor.ProCoSys.PcsServiceBus.Queries;

public class PipeTestQuery
{
    public static string GetQuery(long? revisionId,long? pipeTestLibraryId, string plant = null)
    {
        DetectFaultyPlantInput(plant);
        var whereClause = CreateWhereClause(revisionId, pipeTestLibraryId, plant);

        return @$"select
        '{{""Plant"" : ""' || pt.projectschema ||
        '"", ""PipingRevision"" : ""' || pr.testrevisionno ||
        '"", ""PipeTestLibrary"" : ""' || ptl.label ||
        '"", ""ChecklistId"" : ""' || pt.tagcheck_id ||
        '"", ""LastUpdated"" : ""' || TO_CHAR(pt.last_updated, 'yyyy-mm-dd hh24:mi:ss') ||
        '""}}' as message
        from pipetest pt
            join pipingrevision pr on pr.pipingrevision_id = pt.pipingrevision_id      
            join pipetestlibrary ptl on ptl.pipetestlibrary_id = pt.pipetestlibrary_id  
        {whereClause}";
    }

    private static string CreateWhereClause(long? revisionId, long? pipeTestLibraryId, string plant)
    {
        var whereClause = "";
        if (revisionId is not null & pipeTestLibraryId is not null & plant is not null)
        {
            whereClause = $"where pt.projectschema = '{plant}' and pt.pipingrevision_id = {revisionId} and pt.pipetestlibrary_id = {pipeTestLibraryId}";
        }
        else if (plant is not null)
        {
            whereClause = $"where pt.projectschema = '{plant}'";
        }
        else if (revisionId is not null & pipeTestLibraryId is not null)
        {
            whereClause = $"where and pt.pipingrevision_id = {revisionId} and pt.pipetestlibrary_id = {pipeTestLibraryId}";
        }
        else if (revisionId is not null ^ pipeTestLibraryId is not null)
        {
            throw new Exception("Message can not contain partial id match, need both milestone and wo id to find correct db entry");
        }
        return whereClause;
    }
}
