using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IWorkOrderMilestoneEventV1 : IHasEventType
{
    string Plant { get; }
    Guid ProCoSysGuid { get; }
    string ProjectName { get; }
    long WoId { get; }
    Guid? WoGuid { get; }
    string WoNo { get; }
    string Code { get; }
    DateOnly? MilestoneDate { get; }
    string? SignedByAzureOid { get; }
    DateTime LastUpdated { get; }
}
