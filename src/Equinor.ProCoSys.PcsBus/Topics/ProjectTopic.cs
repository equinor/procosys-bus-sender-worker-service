using System;

namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
public class ProjectTopic
{
    public const string TopicName = "project";
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string Behavior { get; set; }
    public string ProjectName { get; set; }
    public string Description { get; set; }
    public bool IsClosed { get; set; }
}
