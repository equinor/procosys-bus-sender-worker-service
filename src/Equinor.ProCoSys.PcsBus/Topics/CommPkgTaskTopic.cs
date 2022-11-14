using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class CommPkgTaskTopic
{
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string TaskGuid { get; set; }
    public string CommPkgGuid { get; set; }
    public string CommPkgNo { get; set; }
    public string LastUpdated { get; set; }

    public const string TopicName = "commpkgtask";
}
