using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IWorkOrderChecklistEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    long ChecklistId { get; init; }
    Guid ChecklistGuid { get; init; }
    long WoId { get; init; }
    Guid WoGuid { get; init; }
    string WoNo { get; init; }
    DateTime LastUpdated { get; init; }
}
