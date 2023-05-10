using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IActionEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    Guid ElementContentGuid { get; init; }
    string CommPkgNo { get; init; }
    Guid? CommPkgGuid { get; init; }
    string? SwcrNo { get; init; }
    Guid? SwcrGuid { get; init; }
    string? DocumentNo { get; init; }
    string? Description { get; init; }
    Guid? DocumentGuid { get; init; }
    string ActionNo { get; init; }
    string? Title { get; init; }
    string? Comments { get; init; }
    DateOnly? Deadline { get; init; }
    string? CategoryCode { get; init; }
    Guid? CategoryGuid { get; init; }
    string? PriorityCode { get; init; }
    Guid? PriorityGuid { get; init; }
    Guid? RequestedByOid { get; init; }
    Guid? ActionByOid { get; init; }
    string? ActionByRole { get; init; }
    Guid? ActionByRoleGuid { get; init; }
    Guid? ResponsibleOid { get; init; }
    string? ResponsibleRole { get; init; }
    Guid? ResponsibleRoleGuid { get; init; }
    DateTime LastUpdated { get; init; }
    DateTime? SignedAt { get; init; }
    Guid? SignedBy { get; init; }
}
