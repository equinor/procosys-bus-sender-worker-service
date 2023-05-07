using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IQueryEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    long QueryId { get; init; }
    string QueryNo { get; init; }
    string? Title { get; init; }
    string? DisciplineCode { get; init; }
    string? QueryType { get; init; }
    string? CostImpact { get; init; }
    string? Description { get; init; }
    string? Consequence { get; init; }
    string? ProposedSolution { get; init; }
    string? EngineeringReply { get; init; }
    string? Milestone { get; init; }
    bool ScheduleImpact { get; init; }
    bool PossibleWarrantyClaim { get; init; }
    bool IsVoided { get; init; }
    DateOnly? RequiredDate { get; init; }
    DateTime CreatedAt { get; init; }
    DateTime LastUpdated { get; init; }
}
