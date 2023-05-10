namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
public class SwcrAttachmentTopic
{
    public const string TopicName = "swcrattachment";
    public string Plant { get; set; }
    public string ProCoSysGuid { get; set; }
    public string SwcrGuid { get; set; }
    public string Title { get; set; }
    public string ClassificationCode { get; set; }
    public string URI { get; set; }
    public string FileName { get; set; }
}
