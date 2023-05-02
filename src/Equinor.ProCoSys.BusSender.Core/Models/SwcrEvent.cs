using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class SwcrEvent : ISwcrEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string? ProjectName { get; set; }
    public string SwcrNo { get; set; }
    public long SwcrId { get; set; }
    public Guid? CommPkgGuid { get; set; }
    public string? CommPkgNo { get; set; }
    public string? Description { get; set; }
    public string? Modification { get; set; }
    public string? Priority { get; set; }
    public string? System { get; set; }
    public string? ControlSystem { get; set; }
    public string? Contract { get; set; }
    public string? Supplier { get; set; }
    public string? Node { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsVoided { get; set; }
    public DateTime LastUpdated { get; set; }
    public DateOnly? DueDate { get; set; }
    public float? EstimatedManHours { get; set; }
    
    public string EventType  => PcsEventConstants.SwcrCreateOrUpdate;
}
