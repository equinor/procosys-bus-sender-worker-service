namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618

public class SwcrOtherReferencesTopic
{
    public string Plant { get; set; }
    public string ProCoSysGuid { get; set; }
    public string LibraryGuid { get; set; }
    public string SwcrGuid { get; set; }
    public string Code { get; set; }
    public string CodeDescription { get; set; }
    public string LastUpdated { get; set; }

    public const string TopicName = "swcrotherreference";
}
