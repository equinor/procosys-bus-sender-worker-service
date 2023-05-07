using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IDocumentEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    long DocumentId { get; init; }   
    string DocumentNo { get; init; }
    string? Title { get; init; }
    string? AcceptanceCode { get; init; }
    string? Archive { get; init; }
    string? AccessCode { get; init; }
    string? Complex { get; init; }
    string? DocumentType { get; init; }
    string? DisciplineId { get; init; }
    string? DocumentCategory { get; init; }
    string? HandoverStatus { get; init; }
    string? RegisterType { get; init; }
    int? RevisionNo { get; init; }
    string? RevisionStatus { get; init; }
    string? ResponsibleContractor { get; init; }
    DateTime LastUpdated { get; init; }
    DateOnly? RevisionDate { get; init; }
}
