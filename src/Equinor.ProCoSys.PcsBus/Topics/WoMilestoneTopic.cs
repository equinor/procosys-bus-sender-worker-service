
namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class WoMilestoneTopic
{
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string WoNo { get; set; }
    public string Code { get; set; }
    public string MilestoneDate { get; set; }
    public string SignedByAzureOid { get; set; }
    public string LastUpdated { get; set; }

    public const string TopicName = "womilestone";

}
