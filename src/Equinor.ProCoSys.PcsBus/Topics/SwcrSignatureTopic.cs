using System;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class SwcrSignatureTopic
    {
        public string Node { get; set; }
        public string SWCRNO { get; set; }
        public string SignatureRoleCode { get; set; }
        public string Sequence { get; set; }
        public string FunctionalRoleCode { get; set; }
        public DateTime SignedDate { get; set; }
        public DateTime SignedByAzureOid { get; set; }

        public const string TopicName = "swcrsignature";
    }
}
