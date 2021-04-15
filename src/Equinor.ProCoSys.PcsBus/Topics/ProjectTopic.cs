namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class ProjectTopic
    {
        public string Plant { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string IsClosed { get; set; }
        public const string TopicName = "project";
    }
}
