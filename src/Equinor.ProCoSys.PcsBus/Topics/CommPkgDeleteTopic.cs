using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class CommPkgDeleteTopic
{
    public string Plant { get; set; }
    public string CommPkgNo { get; set; }
    public string CommPkgId { get; set; }

    public const string TopicName = "commpkg_delete";
}
