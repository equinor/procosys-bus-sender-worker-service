using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IResponsibleEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    long ResponsibleId { get; init; }
    string Code { get; init; }
    string ResponsibleGroup { get; init; }
    string? Description { get; init; }
    bool IsVoided { get; init; }
    DateTime LastUpdated { get; init; }
    
}
