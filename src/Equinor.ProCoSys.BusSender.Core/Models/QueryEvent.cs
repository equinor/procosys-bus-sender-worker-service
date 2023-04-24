using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class QueryEvent : IQueryEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public long QueryId { get; set; }
    public string QueryNo { get; set; }
    public string? Title { get; set; }
    public string? DisciplineCode { get; set; }
    public string? QueryType { get; set; }
    public string? CostImpact { get; set; }
    public string? Description { get; set; }
    public string? Consequence { get; set; }
    public string? ProposedSolution { get; set; }
    public string? EngineeringReply { get; set; }
    public string? Milestone { get; set; }
    public bool ScheduleImpact { get; set; }
    public bool PossibleWarrantyClaim { get; set; }
    public bool IsVoided { get; set; }
    public DateOnly? RequiredDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastUpdated { get; set; }
    public string EventType => PcsEventConstants.QueryCreateOrUpdate;
}
