using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable 8618

public class PunchListItemEvent : IPunchListItemEventV1
{
    public string EventType => PcsEventConstants.PunchListItemCreateOrUpdate;
    public string Category { get; init; }
    public Guid ChecklistGuid { get; init; }
    public long ChecklistId { get; init; }
    public DateTime? ClearedAt { get; init; }
    public string? ClearingByOrg { get; init; }
    public Guid? ClearingByOrgGuid { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? Description { get; init; }
    public Guid? DocumentGuid { get; init; }
    public string? DocumentNo { get; init; }
    public DateTime? DueDate { get; init; }
    public string? Estimate { get; init; }
    public string? ExternalItemNo { get; init; }
    public bool IsVoided { get; init; }
    public DateTime LastUpdated { get; init; }
    public DateTime? MaterialETA { get; init; }
    public string? MaterialExternalNo { get; init; }
    public bool MaterialRequired { get; init; }
    public Guid? OriginalWoGuid { get; init; }
    public string? OriginalWoNo { get; init; }
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public long PunchItemNo { get; init; }
    public string? PunchListSorting { get; init; }
    public Guid? SortingGuid { get; init; }
    public string? PunchListType { get; init; }
    public Guid? TypeGuid { get; init; }
    public string? PunchPriority { get; init; }
    public Guid? PriorityGuid { get; init; }
    public string? RaisedByOrg { get; init; }
    public Guid? RaisedByOrgGuid { get; init; }
    public DateTime? RejectedAt { get; init; }
    public Guid? SWCRGuid { get; init; }
    public string? SWCRNo { get; init; }
    public DateTime? VerifiedAt { get; init; }
    public Guid? WoGuid { get; init; }
    public string? WoNo { get; init; }
}
