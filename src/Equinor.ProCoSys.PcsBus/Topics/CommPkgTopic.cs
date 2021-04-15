namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class CommPkgTopic
    {
        public string Plant { get; set; }
        public string ProjectName { get; set; }
        public string ProjectNameOld { get; set; }
        public string CommPkgNo { get; set; }
        public string Description { get; set; }
        public const string TopicName = "commpkg";
    }
}
