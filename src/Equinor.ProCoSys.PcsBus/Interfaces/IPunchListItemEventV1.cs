using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;


[UsedImplicitly]
public interface IPunchListItemEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    DateTime LastUpdated { get; init; }
    long PunchItemNo { get; init; }
    string? Description { get; init; }
    long ChecklistId { get; init; }
    Guid ChecklistGuid { get; init; }
    string Category { get; init; }
    string? RaisedByOrg { get; init; }
    string? ClearingByOrg { get; init; }
    DateTime? DueDate { get; init; }
    string? PunchListSorting { get; init; }
    string? PunchListType { get; init; }
    string? PunchPriority { get; init; }
    string? Estimate { get; init; }
    string? OriginalWoNo { get; init; }
    Guid? OriginalWoGuid { get; init; }
    string? WoNo { get; init; }
    Guid? WoGuid { get; init; }
    string? SWCRNo { get; init; }
    Guid? SWCRGuid { get; init; }
    string? DocumentNo { get; init; }
    Guid? DocumentGuid { get; init; }
    string? ExternalItemNo { get; init; }
    bool MaterialRequired { get; init; }
    bool IsVoided { get; init; }
    DateTime? MaterialETA { get; init; }
    string? MaterialExternalNo { get; init; }
    DateTime? ClearedAt { get; init; }
    DateTime? RejectedAt { get; init; }
    DateTime? VerifiedAt { get; init; }
    DateTime CreatedAt { get; init; }
}
