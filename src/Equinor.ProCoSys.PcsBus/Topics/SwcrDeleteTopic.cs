namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class SwcrDeleteTopic
    {
        public string Plant { get; set; }
        public string ProjectName { get; set; }
        public string SWCRNO { get; set; }
        public string SWCRId { get; set; }

        public const string TopicName = "swcr_delete";
    }
}
