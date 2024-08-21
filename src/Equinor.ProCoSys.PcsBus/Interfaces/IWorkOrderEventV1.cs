using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IWorkOrderEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    string WoNo { get; init; }
    long WoId { get; init; }
    string? CommPkgNo { get; init; }
    Guid? CommPkgGuid { get; init; }
    string? Title { get; init; }
    string? Description { get; init; }
    string? MilestoneCode { get; init; }
    string? SubMilestoneCode { get; init; }
    string? CategoryCode { get; init; }
    string? MaterialStatusCode { get; init; }
    string? HoldByCode { get; init; }
    string? DisciplineCode { get; init; }
    string? ResponsibleCode { get; init; }
    string? AreaCode { get; init; }
    string? JobStatusCode { get; init; }
    string? MaterialComments { get; init; }
    string? ConstructionComments { get; init; }
    string? TypeOfWorkCode { get; init; }
    string? OnShoreOffShoreCode { get; init; }
    string? WoTypeCode { get; init; }
    double ProjectProgress { get; init; }
    int Progress { get; init; }
    string? ExpendedManHours { get; init; }
    string? EstimatedHours { get; init; }
    string? RemainingHours { get; init; }
    string? WBS { get; init; }
    DateOnly? PlannedStartAtDate { get; init; }
    DateOnly? ActualStartAtDate { get; init; }
    DateOnly? PlannedFinishedAtDate { get; init; }
    DateOnly? ActualFinishedAtDate { get; init; }
    DateTime CreatedAt { get; init; }
    bool IsVoided { get; init; }
    DateTime LastUpdated { get; init; }
}
