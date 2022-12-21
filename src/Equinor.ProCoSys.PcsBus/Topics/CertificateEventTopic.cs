using System;

namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class CertificateEventTopic
{
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string CertificateType { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string EventType { get; set; }
    public const string TopicName = "certificateevent";
}
