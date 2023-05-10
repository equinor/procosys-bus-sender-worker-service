namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
public class StockTopic
{
    public const string TopicName = "stock";
    public string Plant { get; set; }
    public string StockId { get; set; }
    public string StockNo { get; set; }
    public string Description { get; set; }
    public string LastUpdated { get; set; }
}
