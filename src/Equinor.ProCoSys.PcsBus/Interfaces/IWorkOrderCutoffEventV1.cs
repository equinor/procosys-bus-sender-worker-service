using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IWorkOrderCutoffEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string PlantName { get; init; }
    Guid WoGuid { get; init; }
    string ProjectName { get; init; }
    string WoNo { get; init; }
    string? JobStatusCode { get; init; }
    string? MaterialStatusCode { get; init; }
    string? DisciplineCode { get; init; }
    string? CategoryCode { get; init; }
    string? MilestoneCode { get; init; }
    string? SubMilestoneCode { get; init; }
    string? HoldByCode { get; init; }
    string? PlanActivityCode { get; init; }
    string? ResponsibleCode { get; init; }
    DateTime LastUpdated { get; init; }
    int CutoffWeek { get; init; }
    DateOnly CutoffDate { get; init; }
    DateOnly? PlannedStartAtDate { get; init; }
    DateOnly? PlannedFinishedAtDate { get; init; }
    double? ExpendedManHours { get; init; }
    double? ManHoursEarned { get; init; }
    double? EstimatedHours { get; init; }
    double? ManHoursExpendedLastWeek { get; init; }
    double? ManHoursEarnedLastWeek { get; init; }
    double? ProjectProgress { get; init; }
}
