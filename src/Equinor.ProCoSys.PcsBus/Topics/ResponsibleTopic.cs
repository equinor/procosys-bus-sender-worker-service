namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class ResponsibleTopic
    {
        public string Plant { get; set; }
        public string ResponsibleGroup { get; set; }
        public string ResponsibleGroupOld { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string CodeOld { get; set; }
        public const string TopicName = "responsible";
    }
}
