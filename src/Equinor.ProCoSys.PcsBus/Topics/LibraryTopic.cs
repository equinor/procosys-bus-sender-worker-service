
namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class LibraryTopic
    {
        public string Plant { get; set; }
        public string Code { get; set; }
        public string CodeOld { get; set; }
        public string Description { get; set; }
        public bool IsVoided { get; set; }
        public string Type { get; set; } //for now only triggers on FUNCTIONAL_ROLE type
        public string LastUpdated { get; set; }

        public const string TopicName = "library";
    }
}
