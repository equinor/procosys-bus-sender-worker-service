using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class McPkgDeleteTopic
    {
        public string Plant { get; set; }
        public string ProjectName { get; set; }
        public string McPkgNo { get; set; }
        public string McPkgId { get; set; }

        public const string TopicName = "mcpkg_delete";
    }
}
