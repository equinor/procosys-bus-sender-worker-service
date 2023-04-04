using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class WorkOrderEvent : IWorkOrderEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public string WoNo { get; set; }
    public long WoId { get; set; }
    public string CommPkgNo { get; set; }
    public Guid? CommPkgGuid { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string MilestoneCode { get; set; }
    public string SubMilestoneCode { get; set; }
    public string MilestoneDescription { get; set; }
    public string CategoryCode { get; set; }
    public string MaterialStatusCode { get; set; }
    public string HoldByCode { get; set; }
    public string DisciplineCode { get; set; }
    public string DisciplineDescription { get; set; }
    public string ResponsibleCode { get; set; }
    public string ResponsibleDescription { get; set; }
    public string AreaCode { get; set; }
    public string AreaDescription { get; set; }
    public string JobStatusCode { get; set; }
    public string MaterialComments { get; set; }
    public string ConstructionComments { get; set; }
    public string TypeOfWorkCode { get; set; }
    public string OnShoreOffShoreCode { get; set; }
    public string WoTypeCode { get; set; }
    public string ProjectProgress { get; set; }
    public string ExpendedManHours { get; set; }
    public string EstimatedHours { get; set; }
    public string RemainingHours { get; set; }
    public string? WBS { get; }
    public int Progress { get; set; }
    public DateOnly? PlannedStartAtDate { get; set; }
    public DateOnly? ActualStartAtDate { get; set; }
    public DateOnly? PlannedFinishedAtDate { get; set; }
    public DateOnly? ActualFinishedAtDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsVoided { get; set; }
    public DateTime LastUpdated { get; set; }

    public string EventType => PcsEventConstants.WorkOrderCreateOrUpdate;
}
