using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class DocumentEvent : IDocumentEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public long DocumentId { get; set; }
    public string DocumentNo { get; set; }
    public string? Title { get; set; }
    public string? AcceptanceCode { get; set; }
    public string? Archive { get; set; }
    public string? AccessCode { get; set; }
    public string? Complex { get; set; }
    public string? DocumentType { get; set; }
    public string? DisciplineId { get; set; }
    public string? DocumentCategory { get; set; }
    public string? HandoverStatus { get; set; }
    public string? RegisterType { get; set; }
    public int? RevisionNo { get; set; }
    public string? RevisionStatus { get; set; }
    public string? ResponsibleContractor { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateOnly? RevisionDate { get; set; }
    public string EventType => PcsEventConstants.DocumentCreateOrUpdate;
}
