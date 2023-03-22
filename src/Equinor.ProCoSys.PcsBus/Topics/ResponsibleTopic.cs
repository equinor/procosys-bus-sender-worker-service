using System;

namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
public class ResponsibleTopic
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string Behavior { get; set; }
    public string ResponsibleGroup { get; set; }
    public string ResponsibleGroupOld { get; set; }
    public string Description { get; set; }
    public string Code { get; set; }
    public string CodeOld { get; set; }
    
    public const string TopicName = "responsible";
}
