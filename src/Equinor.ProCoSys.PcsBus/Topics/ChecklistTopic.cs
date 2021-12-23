using System;

namespace Equinor.ProCoSys.PcsServiceBus.Topics
{
    public class ChecklistTopic
    {
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string  ChecklistId { get; set; }
    public string TagNo { get; set; }
    public string TagCategory { get; set; }
    public string FormularGroup { get; set; }
    public string FormularDiscipline { get; set; }
    public string Responsible { get; set; }
    public string Status { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime SignedAt { get; set; }
    public DateTime VerifiedAt { get; set; }
    public const string TopicName = "checklist";
    }
}
