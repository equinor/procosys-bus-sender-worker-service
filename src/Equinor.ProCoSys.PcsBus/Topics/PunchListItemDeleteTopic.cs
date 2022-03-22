using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class PunchListItemDeleteTopic
    {
        public string Plant { get; set; }
        public string PlantName { get; set; }
        public string ProjectName { get; set; }
        public string PunchItemNo { get; set; }
        public string TagNo { get; set; }
        public string ResponsibleCode { get; set; }

        public const string TopicName = "punchlistitem_delete";
    }
}
