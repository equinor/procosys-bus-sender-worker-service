namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class CommPkgTopic
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
        public string ProjectNameOld { get; set; }
        public string CommPkgNo { get; set; }
        public string Description { get; set; }
        public const string TopicName = "commpkg";
    }
}
