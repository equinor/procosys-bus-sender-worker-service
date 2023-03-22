namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
public class SwcrSignatureTopic
{
    public string Node { get; set; }
    public string SWCRNO { get; set; }
    public string SignatureRoleCode { get; set; }
    public string Sequence { get; set; }
    public string FunctionalRoleCode { get; set; }
    public string SignedDate { get; set; }
    public string SignedByAzureOid { get; set; }

    public const string TopicName = "swcrsignature";
}
