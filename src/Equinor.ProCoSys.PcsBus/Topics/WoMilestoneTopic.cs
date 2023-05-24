namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
public class WoMilestoneTopic
{
    public const string TopicName = "womilestone";
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string WoNo { get; set; }
    public string Code { get; set; }
    public string MilestoneDate { get; set; }
    public string SignedByAzureOid { get; set; }
    public string LastUpdated { get; set; }
}
