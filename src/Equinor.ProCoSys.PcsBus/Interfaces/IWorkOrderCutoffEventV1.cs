using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IWorkOrderCutoffEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string PlantName { get; set; }
    Guid WoGuid { get; set; }
    string ProjectName { get; set; }
    string WoNo { get; set; }
    string? JobStatusCode { get; set; }
    string? MaterialStatusCode { get; set; }
    string? DisciplineCode { get; set; }
    string? CategoryCode { get; set; }
    string? MilestoneCode { get; set; }
    string? SubMilestoneCode { get; set; }
    string? HoldByCode { get; set; }
    string? PlanActivityCode { get; set; }
    string? ResponsibleCode { get; set; }
    DateTime LastUpdated { get; set; }
    int CutoffWeek { get; set; }
    DateOnly CutoffDate { get; set; }
    DateOnly? PlannedStartAtDate { get; set; }
    DateOnly? PlannedFinishedAtDate { get; set; }
    double? ExpendedManHours { get; set; }
    double? ManHoursEarned { get; set; }
    double? EstimatedHours { get; set; }
    double? ManHoursExpendedLastWeek { get; set; }
    double? ManHoursEarnedLastWeek { get; set; }
    double? ProjectProgress { get; set; }
}
