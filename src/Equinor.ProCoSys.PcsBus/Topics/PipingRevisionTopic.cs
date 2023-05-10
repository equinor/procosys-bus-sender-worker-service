namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
public class PipingRevisionTopic
{
    public const string TopicName = "piperevision";
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string McPkgNo { get; set; }
    public string Revision { get; set; }
    public string PipingRevisionId { get; set; }
    public string MaxDesignPressure { get; set; }
    public string MaxTestPressure { get; set; }
    public string Comments { get; set; }
    public string TestISODocumentNo { get; set; }
    public string TestISORevision { get; set; }
    public string PurchaseOrderNo { get; set; }
    public string CallOffNo { get; set; }
    public string LastUpdated { get; set; }
}
