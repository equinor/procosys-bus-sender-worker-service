namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class StockTopic
{
    public string Plant { get; set; }
    public string StockId { get; set; }
    public string StockNo { get; set; }
    public string Description { get; set; }
    public string LastUpdated { get; set; }

    public const string TopicName = "stock";
}
