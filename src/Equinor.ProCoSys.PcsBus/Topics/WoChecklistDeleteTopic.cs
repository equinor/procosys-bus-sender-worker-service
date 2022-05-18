namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class WoChecklistDeleteTopic
{
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string WoNo { get; set; }
    public string ChecklistId { get; set; }
 
    public const string TopicName = "wochecklist_delete";
}
