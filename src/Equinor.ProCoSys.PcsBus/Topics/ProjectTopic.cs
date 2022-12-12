using System;

namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class ProjectTopic
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string Behavior { get; set; }
    public string ProjectName { get; set; }
    public string Description { get; set; }
    public bool IsClosed { get; set; }

    public const string TopicName = "project";
}
