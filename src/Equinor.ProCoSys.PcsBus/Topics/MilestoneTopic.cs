using System;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class MilestoneTopic
    {
        public string Plant { get; set; }
        public string ProjectName { get; set; }
        public string CommPkgNo { get; set; }
        public string McPkgNo { get; set; }
        public string Code { get; set; }
        public DateTime ActualDate { get; set; }
        public bool IsSent { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsRejected { get; set; }

        public const string TopicName = "milestone";
    }
}
