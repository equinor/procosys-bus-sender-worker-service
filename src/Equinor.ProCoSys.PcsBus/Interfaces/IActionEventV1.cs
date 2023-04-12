using System;
namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IActionEventV1 :IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    Guid ElementContentGuid { get; set; }
    string CommPkgNo { get; set; }
    Guid? CommPkgGuid { get; set; }
    string? SwcrNo { get; set; }
    Guid? SwcrGuid { get; set; }
    string? DocumentNo { get; set; }
    string? Description { get; set; }
    Guid? DocumentGuid { get; set; }
    string ActionNo { get; set; }
    string? Title { get; set; }
    string? Comments { get; set; }
    DateOnly? Deadline { get; set; }
    string? CategoryCode { get; set; }
    Guid? CategoryGuid { get; set; }
    string? PriorityCode { get; set; }
    Guid? PriorityGuid { get; set; }
    Guid? RequestedByOid { get; set; }
    Guid? ActionByOid { get; set; }
    string? ActionByRole { get; set; }
    Guid? ActionByRoleGuid { get; set; }
    Guid? ResponsibleOid { get; set; }
    string? ResponsibleRole { get; set; }
    Guid? ResponsibleRoleGuid { get; set; }
    DateTime LastUpdated { get; set; }
    DateTime? SignedAt { get; set; }
    Guid? SignedBy { get; set; }
}
