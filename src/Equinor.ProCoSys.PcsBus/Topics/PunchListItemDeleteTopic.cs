namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class PunchListItemDeleteTopic
{
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string PunchItemNo { get; set; }
    public string PunchItemId { get; set; }
    public string ResponsibleCode { get; set; }

    public const string TopicName = "punchlistitem_delete";
}
