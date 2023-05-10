using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class QueryEvent : IQueryEventV1
{
    public string EventType => PcsEventConstants.QueryCreateOrUpdate;
    public string? Consequence { get; init; }
    public string? CostImpact { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? Description { get; init; }
    public string? DisciplineCode { get; init; }
    public string? EngineeringReply { get; init; }
    public bool IsVoided { get; init; }
    public DateTime LastUpdated { get; init; }
    public string? Milestone { get; init; }
    public string Plant { get; init; }
    public bool PossibleWarrantyClaim { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public string? ProposedSolution { get; init; }
    public long QueryId { get; init; }
    public string QueryNo { get; init; }
    public string? QueryType { get; init; }
    public DateOnly? RequiredDate { get; init; }
    public bool ScheduleImpact { get; init; }
    public string? Title { get; init; }
}
