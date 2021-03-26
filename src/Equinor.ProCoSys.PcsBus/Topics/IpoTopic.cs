namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class IpoTopic
    {
        private string _plant;

        public string Plant
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_plant))
                {
                    return ProjectSchema;
                }
                return _plant;
            }
            set => _plant = value;
        }

        public string ProjectSchema { get; set; }
        public string InvitationGuid { get; set; }
        public string Event { get; set; }
        public const string TopicName = "ipo";
        public int Status { get; set; }
    }
}
