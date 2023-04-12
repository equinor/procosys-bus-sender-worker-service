using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class ActionEvent : IActionEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public Guid ElementContentGuid { get; set; }
    public string CommPkgNo { get; set; }
    public Guid? CommPkgGuid { get; set; }
    public string? SwcrNo { get; set; }
    public Guid? SwcrGuid { get; set; }
    public string? DocumentNo { get; set; }
    public string? Description { get; set; }
    public Guid? DocumentGuid { get; set; }
    public string ActionNo { get; set; }
    public string? Title { get; set; }
    public string? Comments { get; set; }
    public DateOnly? Deadline { get; set; }
    public string? CategoryCode { get; set; }
    public Guid? CategoryGuid { get; set; }
    public string? PriorityCode { get; set; }
    public Guid? PriorityGuid { get; set; }
    public Guid? RequestedByOid { get; set; }
    public Guid? ActionByOid { get; set; }
    public string? ActionByRole { get; set; }
    public Guid? ActionByRoleGuid { get; set; }
    public Guid? ResponsibleOid { get; set; }
    public string? ResponsibleRole { get; set; }
    public Guid? ResponsibleRoleGuid { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateTime? SignedAt { get; set; }
    public Guid? SignedBy { get; set; }
    
    public string EventType => PcsEventConstants.ActionCreateOrUpdate;
}
