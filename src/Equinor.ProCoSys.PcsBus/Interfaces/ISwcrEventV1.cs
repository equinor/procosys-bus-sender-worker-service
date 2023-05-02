using System;
namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ISwcrEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string? ProjectName { get; set; }
    string SwcrNo { get; set; }
    long SwcrId { get; set; }
    Guid? CommPkgGuid { get; set; }
    string? CommPkgNo { get; set; }
    string? Description { get; set; }
    string? Modification { get; set; }
    string? Priority { get; set; }
    string? System { get; set; }
    string? ControlSystem { get; set; }
    string? Contract { get; set; }
    string? Supplier { get; set; }
    string? Node { get; set; }
    string? Status { get; set; }
    DateTime CreatedAt { get; set; }
    bool IsVoided { get; set; }
    DateTime LastUpdated { get; set; }
    DateOnly? DueDate { get; set; }
    float? EstimatedManHours { get; set; }
}
