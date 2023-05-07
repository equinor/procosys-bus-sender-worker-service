using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface ICommPkgOperationEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    string CommPkgNo { get; init; }
    Guid CommPkgGuid { get; init; }
    bool InOperation { get; init; }
    bool ReadyForProduction { get; init; }
    bool MaintenanceProgram { get; init; }
    bool YellowLine { get; init; }
    bool BlueLine { get; init; }
    string? YellowLineStatus { get; init; }
    string? BlueLineStatus { get; init; }
    bool TemporaryOperationEst { get; init; }
    bool PmRoutine { get; init; }
    bool CommissioningResp { get; init; }
    bool? ValveBlindingList { get; init; }
    DateTime LastUpdated { get; init; }
}
