namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class PipingRevisionDeleteTopic
{
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string PipingRevisionId { get; set; }
    public string Revision { get; set; }
    public string McPkgNo { get; set; }

    public const string TopicName = "pipingrevision_delete";
}
