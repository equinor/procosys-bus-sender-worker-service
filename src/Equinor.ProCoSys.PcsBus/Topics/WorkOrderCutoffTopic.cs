namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
public class WorkOrderCutoffTopic
{
    public string Plant { get; set; }
    public string PlantName { get; set; }
    public string ProjectName { get; set; }
    public string WoNo { get; set; }
    public string MileStoneCode { get; set; }
    public string SubMilestoneCode { get; set; }
    public string MaterialStatusCode { get; set; }
    public string CategoryCode { get; set; }
    public string HoldByCode { get; set; }
    public string ResponsibleCode { get; set; }
    public string JobStatusCode { get; set; }
    public string WoTypeCode { get; set; }
    public string ProjectProgress { get; set; }
    public string PlannedStartAtDate { get; set; }
    public string UpdatedAtDate { get; set; }
    public string PlannedFinishAtDate { get; set; }
    public string ExpendedManHours { get; set; }
    public string ManHoursEarned { get; set; }
    public string ManHoursExpendedLastWeek { get; set; }
    public string ManHoursEarnedLastWeek { get; set; }

    public const string TopicName = "wocutoff";
}
