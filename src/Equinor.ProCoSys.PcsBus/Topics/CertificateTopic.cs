using System;
using Equinor.ProCoSys.PcsServiceBus.Enums;

namespace Equinor.ProCoSys.PcsServiceBus.Topics;
#pragma warning disable CS8618
// todo This Topic is used for "accept of a certificate" (consumed in preservation).
// Can't be reused as-is for "create certificate" or "edit certificate". See #98630
public class CertificateTopic
{
    public const string TopicName = "certificate";
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string CertificateNo { get; set; }
    public string CertificateType { get; set; }
    public CertificateStatus CertificateStatus { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string Behavior { get; set; }
}
