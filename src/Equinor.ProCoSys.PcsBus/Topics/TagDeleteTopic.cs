using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class TagDeleteTopic
    {
        public string Plant { get; set; }
        public string PlantName { get; set; }
        public string TagId { get; set; }
        public string TagNo { get; set; }
        public string ProjectName { get; set; }
        public string AreaCode { get; set; }
        public string RegisterCode { get; set; }
        public string Status { get; set; }
        public string System { get; set; }
        public string TagFunctionCode { get; set; }

        public const string TopicName = "tag_delete";
    }
}
