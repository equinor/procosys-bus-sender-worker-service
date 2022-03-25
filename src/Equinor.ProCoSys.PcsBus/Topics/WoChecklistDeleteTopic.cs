namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class WoChecklistDeleteTopic
    {
        public string Plant { get; set; }
        public string ProjectName { get; set; }
        public string WoNo { get; set; }
        public string TagNo { get; set; }
        public string TagId { get; set; }
        public string FormularType { get; set; }
        public string FormularGroup { get; set; }
        public string Revision { get; set; }
        public string Responsible { get; set; }

        public const string TopicName = "wochecklist_delete";
    }
}
