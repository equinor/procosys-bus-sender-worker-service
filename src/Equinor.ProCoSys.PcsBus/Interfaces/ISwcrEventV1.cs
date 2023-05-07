using System;
namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ISwcrEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string? ProjectName { get; init; }
    string SwcrNo { get; init; }
    long SwcrId { get; init; }
    Guid? CommPkgGuid { get; init; }
    string? CommPkgNo { get; init; }
    string? Description { get; init; }
    string? Modification { get; init; }
    string? Priority { get; init; }
    string? System { get; init; }
    string? ControlSystem { get; init; }
    string? Contract { get; init; }
    string? Supplier { get; init; }
    string? Node { get; init; }
    string? Status { get; init; }
    DateTime CreatedAt { get; init; }
    bool IsVoided { get; init; }
    DateTime LastUpdated { get; init; }
    DateOnly? DueDate { get; init; }
    float? EstimatedManHours { get; init; }
}
