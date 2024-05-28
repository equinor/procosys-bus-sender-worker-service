﻿using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class DocumentEvent : IDocumentEventV1
{
    public string? AcceptanceCode { get; init; }
    public string? AccessCode { get; init; }
    public string? Archive { get; init; }
    public string? Complex { get; init; }
    public string? DisciplineId { get; init; }
    public string? DocumentCategory { get; init; }
    public long DocumentId { get; init; }
    public string DocumentNo { get; init; }
    public string? DocumentType { get; init; }
    public string? HandoverStatus { get; init; }
    public DateTime LastUpdated { get; init; }
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public string? RegisterType { get; init; }
    public string? ResponsibleContractor { get; init; }
    public DateOnly? RevisionDate { get; init; }
    public string? RevisionNo { get; init; }
    public string? RevisionStatus { get; init; }
    public string? Title { get; init; }
    public string EventType => PcsEventConstants.DocumentCreateOrUpdate;
    public bool IsVoided { get; init; }
}
