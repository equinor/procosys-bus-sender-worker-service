using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

#pragma warning disable CS8618
internal class ChecklistEvent : IChecklistEventV1
{
    public string Plant { get;  }
    public Guid ProCoSysGuid { get;  }
    public string ProjectName { get; }
    public string TagNo { get; }
    public long TagId { get; }
    public Guid TagGuid { get; }
    public long TagRegisterId { get; }
    public long ChecklistId { get; }
    public string TagCategory { get; }
    public string SheetNo { get; }
    public string SubSheetNo { get; }
    public string FormularType { get; }
    public string FormularGroup { get; }
    public string FormPhase { get; }
    public string SystemModule { get; }
    public string FormularDiscipline { get; }
    public string Revision { get; }
    public string? PipingRevisionMcPkNo { get; }
    public Guid? PipingRevisionMcPkGuid { get; }
    public string Responsible { get; }
    public string Status { get; }
    public DateTime? UpdatedAt { get; }
    public DateTime LastUpdated { get; }
    public DateTime CreatedAt { get; }
    public DateTime? SignedAt { get; }
    public DateTime? VerifiedAt { get; }

    public string EventType => PcsEventConstants.ChecklistCreateOrUpdate;
}
