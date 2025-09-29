using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public sealed record DocumentEvent(
    string Plant,
    Guid ProCoSysGuid,
    string? ProjectName,
    long DocumentId,
    string DocumentNo,
    string? Title,
    string? AcceptanceCode,
    string? Archive,
    string? AccessCode,
    string? Complex,
    string? DocumentType,
    string? DisciplineId,
    string? DocumentCategory,
    string? HandoverStatus,
    Guid RegisterGuid,
    string? RegisterType,
    string? RevisionNo,
    string? RevisionStatus,
    string? ResponsibleContractor,
    DateTime LastUpdated,
    DateOnly? RevisionDate,
    bool IsVoided,
    string? InstallationCode) : IDocumentEventV1
{
    public string EventType => PcsEventConstants.DocumentCreateOrUpdate;
}
