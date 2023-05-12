using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IWorkOrderMilestoneEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    long WoId { get; init; }
    Guid? WoGuid { get; init; }
    string WoNo { get; init; }
    string Code { get; init; }
    DateOnly? MilestoneDate { get; init; }
    string? SignedByAzureOid { get; init; }
    DateTime LastUpdated { get; init; }
}
