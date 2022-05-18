using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class TagDeleteTopic
{
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string TagNo { get; set; }
    public string TagId { get; set; }

    public const string TopicName = "tag_delete";
}
