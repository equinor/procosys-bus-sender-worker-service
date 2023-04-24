using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IQueryEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string ProjectName { get; set; }
    long QueryId { get; set; }
    string QueryNo { get; set; }
    string? Title { get; set; }
    string? DisciplineCode { get; set; }
    string? QueryType { get; set; }
    string? CostImpact { get; set; }
    string? Description { get; set; }
    string? Consequence { get; set; }
    string? ProposedSolution { get; set; }
    string? EngineeringReply { get; set; }
    string? Milestone { get; set; }
    bool ScheduleImpact { get; set; }
    bool PossibleWarrantyClaim { get; set; }
    bool IsVoided { get; set; }
    DateOnly? RequiredDate { get; set; }
    DateTime CreatedAt { get; set; }
    DateTime LastUpdated { get; set; }
}
