using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

#pragma warning disable CS8618

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class ActionEvent : IActionEventV1
{
    public Guid? ActionByOid { get; init; }
    public string? ActionByRole { get; init; }
    public Guid? ActionByRoleGuid { get; init; }
    public string ActionNo { get; init; }
    public string? CategoryCode { get; init; }
    public Guid? CategoryGuid { get; init; }
    public string? Comments { get; init; }
    public Guid? CommPkgGuid { get; init; }
    public string CommPkgNo { get; init; }
    public DateOnly? Deadline { get; init; }
    public string? Description { get; init; }
    public Guid? DocumentGuid { get; init; }
    public string? DocumentNo { get; init; }
    public Guid ElementContentGuid { get; init; }
    public DateTime LastUpdated { get; init; }
    public string Plant { get; init; }
    public string? PriorityCode { get; init; }
    public Guid? PriorityGuid { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public Guid? RequestedByOid { get; init; }
    public Guid? ResponsibleOid { get; init; }
    public string? ResponsibleRole { get; init; }
    public Guid? ResponsibleRoleGuid { get; init; }
    public DateTime? SignedAt { get; init; }
    public Guid? SignedBy { get; init; }
    public Guid? SwcrGuid { get; init; }
    public string? SwcrNo { get; init; }
    public string? Title { get; init; }

    public string EventType => PcsEventConstants.ActionCreateOrUpdate;
}
