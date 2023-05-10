using System;

namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
public class LibraryTopic
{
    public const string TopicName = "library";
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string Behavior { get; set; }
    public string Code { get; set; }
    public string CodeOld { get; set; }
    public string LibraryId { get; set; }
    public string ParentId { get; set; }
    public string Description { get; set; }
    public bool IsVoided { get; set; }
    public string Type { get; set; } //for now only triggers on FUNCTIONAL_ROLE type
    public string LastUpdated { get; set; }
}
