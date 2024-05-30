﻿using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IPunchListItemEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    Guid ProjectGuid { get; init; }
    Guid? ModifiedByGuid { get; init; }
    DateTime LastUpdated { get; init; }
    long PunchItemNo { get; init; }
    string? Description { get; init; }
    long ChecklistId { get; init; }
    Guid ChecklistGuid { get; init; }
    string Category { get; init; }
    string? RaisedByOrg { get; init; }
    Guid? RaisedByOrgGuid { get; init; }
    string? ClearingByOrg { get; init; }
    Guid? ClearingByOrgGuid { get; init; }
    DateTime? DueDate { get; init; }
    string? PunchListSorting { get; init; }
    Guid? PunchListSortingGuid { get; init; }
    string? PunchListType { get; init; }
    Guid? PunchListTypeGuid { get; init; }
    string? PunchPriority { get; init; }
    Guid? PunchPriorityGuid { get; init; }
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
    Guid? ClearedByGuid { get; init; }
    DateTime? ClearedAt { get; init; }
    Guid? RejectedByGuid { get; init; }
    DateTime? RejectedAt { get; init; }
    Guid? VerifiedByGuid { get; init; }
    DateTime? VerifiedAt { get; init; }
    Guid CreatedByGuid { get; init; }
    DateTime CreatedAt { get; init; }
    Guid? ActionByGuid { get; init; }



}
