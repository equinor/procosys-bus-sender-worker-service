namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class DocumentTopic
{
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string DocumentId { get; set; }
    public string DocumentNo { get; set; }
    public string Title { get; set; }
    public string AcceptanceCode { get; set; }
    public string Archive { get; set; }
    public string AccessCode { get; set; }
    public string Complex { get; set; }
    public string DocumentType { get; set; }
    public string DisciplineId { get; set; }
    public string DocumentCategory { get; set; }
    public string HandoverStatus { get; set; }
    public string RegisterType { get; set; }
    public string RevisionNo { get; set; }
    public string RevisionStatus { get; set; }
    public string ResponsibleContractor { get; set; }
    public string LastUpdated { get; set; }
    public string RevisionDate { get; set; }

    public const string TopicName = "document";
}
