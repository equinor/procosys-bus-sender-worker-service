
namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class WoMaterialDeleteTopic
{
    public string Plant { get; set; }
    public string WoNo { get; set; }
    public string ItemNo { get; set; }

    public const string TopicName = "womaterial_delete";
}
