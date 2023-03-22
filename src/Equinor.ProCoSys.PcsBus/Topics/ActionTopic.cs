namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
public class ActionTopic
{
    public string Plant { get; set; }
    public string ProCoSysGuid { get; set; }
    public string ElementContentGuid { get; set; }
    public string ActionNo { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string CommPkgNo { get; set; }
    public string CommPkgGuid { get; set; }
    public string LastUpdated { get; set; }
    public string SignedAt { get; set; }
    public string SignedBy { get; set; }

    public const string TopicName = "action";
}
