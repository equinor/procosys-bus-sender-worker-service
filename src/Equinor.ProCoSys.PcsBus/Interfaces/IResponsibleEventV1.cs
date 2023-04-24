using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IResponsibleEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    long ResponsibleId { get; set; }
    string Code { get; set; }
    string ResponsibleGroup { get; set; }
    string? Description { get; set; }
    bool IsVoided { get; set; }
    DateTime LastUpdated { get; set; }
    
}
