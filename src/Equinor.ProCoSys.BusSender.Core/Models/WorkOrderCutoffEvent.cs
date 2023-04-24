using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class WorkOrderCutoffEvent : IWorkOrderCutoffEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string PlantName { get; set; }
    public Guid WoGuid { get; set; }
    public string ProjectName { get; set; }
    public string WoNo { get; set; }
    public string? JobStatusCode { get; set; }
    public string? MaterialStatusCode { get; set; }
    public string? DisciplineCode { get; set; }
    public string? CategoryCode { get; set; }
    public string? MilestoneCode { get; set; }
    public string? SubMilestoneCode { get; set; }
    public string? HoldByCode { get; set; }
    public string? PlanActivityCode { get; set; }
    public string? ResponsibleCode { get; set; }
    public DateTime LastUpdated { get; set; }
    public int CutoffWeek { get; set; }
    public DateOnly CutoffDate { get; set; }
    public DateOnly? PlannedStartAtDate { get; set; }
    public DateOnly? PlannedFinishedAtDate { get; set; }
    public double? ExpendedManHours { get; set; }
    public double? ManHoursEarned { get; set; }
    public double? EstimatedHours { get; set; }
    public double? ManHoursExpendedLastWeek { get; set; }
    public double? ManHoursEarnedLastWeek { get; set; }
    public double? ProjectProgress { get; set; }
    
    public string EventType => PcsEventConstants.WorkOrderCutoffCreateOrUpdate;
}
