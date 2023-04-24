using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IWorkOrderChecklistEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string ProjectName { get; set; }
    long ChecklistId { get; set; }
    Guid ChecklistGuid { get; set; }
    long WoId { get; set; }
    Guid WoGuid { get; set; }
    string WoNo { get; set; }
    DateTime LastUpdated { get; set; }
}
