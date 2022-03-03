namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class WoChecklistTopic
    {
        public string Plant { get; set; }
        public string ProjectName { get; set; }
        public string WoNo { get; set; }
        public string TagNo { get; set; }
        public string FormularType { get; set; }
        public string FormularGroup { get; set; }
        public string Responsible { get; set; }

        public const string TopicName = "wochecklist";
    }
}
