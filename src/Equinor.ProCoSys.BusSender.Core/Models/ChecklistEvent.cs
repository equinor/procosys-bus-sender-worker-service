﻿using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

#pragma warning disable CS8618
public class ChecklistEvent : IChecklistEventV1
{
    public long ChecklistId { get; init; }
    public DateTime CreatedAt { get; init; }
    public string FormPhase { get; init; }
    public string FormularDiscipline { get; init; }
    public string FormularGroup { get; init; }
    public string FormularType { get; init; }
    public DateTime LastUpdated { get; init; }
    public Guid? PipingRevisionMcPkGuid { get; init; }
    public string? PipingRevisionMcPkNo { get; init; }
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public string Responsible { get; init; }
    public string Revision { get; init; }
    public string SheetNo { get; init; }
    public DateTime? SignedAt { get; init; }
    public string Status { get; init; }
    public string SubSheetNo { get; init; }
    public string SystemModule { get; init; }
    public string TagCategory { get; init; }
    public Guid TagGuid { get; init; }
    public long TagId { get; init; }
    public string TagNo { get; init; }
    public long TagRegisterId { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public DateTime? VerifiedAt { get; init; }

    public string EventType => PcsEventConstants.ChecklistCreateOrUpdate;
}
