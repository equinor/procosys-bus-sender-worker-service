using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IDocumentEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string ProjectName { get; set; }
    long DocumentId { get; set; }   
    string DocumentNo { get; set; }
    string? Title { get; set; }
    string? AcceptanceCode { get; set; }
    string? Archive { get; set; }
    string? AccessCode { get; set; }
    string? Complex { get; set; }
    string? DocumentType { get; set; }
    string? DisciplineId { get; set; }
    string? DocumentCategory { get; set; }
    string? HandoverStatus { get; set; }
    string? RegisterType { get; set; }
    int? RevisionNo { get; set; }
    string? RevisionStatus { get; set; }
    string? ResponsibleContractor { get; set; }
    DateTime LastUpdated { get; set; }
    DateOnly? RevisionDate { get; set; }
}
