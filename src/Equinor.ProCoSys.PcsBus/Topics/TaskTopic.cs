namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
public class TaskTopic
{
    public const string TopicName = "task";
    public string Plant { get; set; }
    public string ProCoSysGuid { get; set; }
    public string TaskParentProCoSysGuid { get; set; }
    public string PlantName { get; set; }
    public string ProjectName { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Comments { get; set; }
    public string LastUpdated { get; set; }
    public string SignedAt { get; set; }
    public string SignedBy { get; set; }
}
