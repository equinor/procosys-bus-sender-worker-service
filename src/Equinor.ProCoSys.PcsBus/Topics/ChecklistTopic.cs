using System;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class ChecklistTopic
    {
        public string Plant { get; set; }
        public string ProjectName { get; set; }
        public string ChecklistId { get; set; }
        public string TagNo { get; set; }
        public string TagCategory { get; set; }
        public string FormularGroup { get; set; }
        public string FormularDiscipline { get; set; }
        public string Revision { get; set; }
        public string Responsible { get; set; }
        public string Status { get; set; }
        public string UpdatedAt { get; set; }
        public string SignedAt { get; set; }
        public string VerifiedAt { get; set; }

        public const string TopicName = "checklist";
    }
}
