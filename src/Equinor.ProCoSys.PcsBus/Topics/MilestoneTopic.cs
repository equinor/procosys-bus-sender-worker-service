namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class MilestoneTopic
    {
        public string Plant { get; set; }
        public string ProjectName { get; set; }
        public string CommPkgNo { get; set; }
        public string McPkgNo { get; set; }
        public string Code { get; set; }
        public string ActualDate { get; set; }
        public string IsSent { get; set; }
        public string IsAccepted { get; set; }
        public string IsRejected { get; set; }

        public const string TopicName = "milestone";
    }
}
