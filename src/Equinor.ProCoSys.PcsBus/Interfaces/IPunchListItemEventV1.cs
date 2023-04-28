using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;


[UsedImplicitly]
public interface IPunchListItemEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string ProjectName { get; set; }
    DateTime LastUpdated { get; set; }
    long PunchItemNo { get; set; }
    string? Description { get; set; }
    long ChecklistId { get; set; }
    Guid ChecklistGuid { get; set; }
    string Category { get; set; }
    string? RaisedByOrg { get; set; }
    string? ClearingByOrg { get; set; }
    DateTime? DueDate { get; set; }
    string? PunchListSorting { get; set; }
    string? PunchListType { get; set; }
    string? PunchPriority { get; set; }
    string? Estimate { get; set; }
    string? OriginalWoNo { get; set; }
    Guid? OriginalWoGuid { get; set; }
    string? WoNo { get; set; }
    Guid? WoGuid { get; set; }
    string? SWCRNo { get; set; }
    Guid? SWCRGuid { get; set; }
    string? DocumentNo { get; set; }
    Guid? DocumentGuid { get; set; }
    string? ExternalItemNo { get; set; }
    bool MaterialRequired { get; set; }
    bool IsVoided { get; set; }
    DateTime? MaterialETA { get; set; }
    string? MaterialExternalNo { get; set; }
    DateTime? ClearedAt { get; set; }
    DateTime? RejectedAt { get; set; }
    DateTime? VerifiedAt { get; set; }
    DateTime CreatedAt { get; set; }
}
