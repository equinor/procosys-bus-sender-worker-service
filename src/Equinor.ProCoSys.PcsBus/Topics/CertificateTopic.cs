using Equinor.ProCoSys.PcsServiceBus.Enums;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class CertificateTopic
    {
        public string Plant { get; set; }
        public string ProjectName { get; set; }
        public string CertificateNo { get; set; }
        public string CertificateType { get; set; }
        public CertificateStatus CertificateStatus { get; set; }
        public const string TopicName = "certificate";
    }
}
