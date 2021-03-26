namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class ProjectTopic
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
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string IsClosed { get; set; }
        public const string TopicName = "project";
    }
}
