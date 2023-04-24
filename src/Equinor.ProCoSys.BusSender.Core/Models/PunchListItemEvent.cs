using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable 8618

public class PunchListItemEvent : IPunchListItemEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public DateTime LastUpdated { get; set; }
    public long PunchItemNo { get; set; }
    public string? Description { get; set; }
    public long ChecklistId { get; set; }
    public Guid ChecklistGuid { get; set; }
    public string Category { get; set; }
    public string? RaisedByOrg { get; set; }
    public string? ClearingByOrg { get; set; }
    public DateTime? DueDate { get; set; }
    public string? PunchListSorting { get; set; }
    public string? PunchListType { get; set; }
    public string? PunchPriority { get; set; }
    public string? Estimate { get; set; }
    public string? OriginalWoNo { get; set; }
    public Guid? OriginalWoGuid { get; set; }
    public string? WoNo { get; set; }
    public Guid? WoGuid { get; set; }
    public string? SWCRNo { get; set; }
    public Guid? SWCRGuid { get; set; }
    public string? DocumentNo { get; set; }
    public Guid? DocumentGuid { get; set; }
    public string? ExternalItemNo { get; set; }
    public bool MaterialRequired { get; set; }
    public bool IsVoided { get; set; }
    public DateTime? MaterialETA { get; set; }
    public string? MaterialExternalNo { get; set; }
    public DateTime? ClearedAt { get; set; }
    public DateTime? RejectedAt { get; set; }
    public DateTime? VerifiedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public string EventType => PcsEventConstants.PunchListItemCreateOrUpdate;
}
