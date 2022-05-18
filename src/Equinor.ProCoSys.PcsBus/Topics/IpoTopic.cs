namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class IpoTopic
{
    public string Plant { get; set; }
    public string InvitationGuid { get; set; }
    public string Event { get; set; }
    public const string TopicName = "ipo";
    public int Status { get; set; }
}
