using System;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class SwcrTopic
    {
        public string SWCRNO { get; set; }
        public string SWCRId { get; set; }
        public string CommPkgNo { get; set; }
        public string Description { get; set; }
        public string Modification { get; set; }
        public string Priority { get; set; }
        public string ControlSystem { get; set; }
        public string Contract { get; set; }
        public string Supplier { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime EstimatedManhours { get; set; }
        public string Node { get; set; }

        public const string TopicName = "swcr";
    }
}
