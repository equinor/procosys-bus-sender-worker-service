using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface ICommPkgOperationEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string ProjectName { get; set; }
    string CommPkgNo { get; set; }
    Guid CommPkgGuid { get; set; }
    bool InOperation { get; set; }
    bool ReadyForProduction { get; set; }
    bool MaintenanceProgram { get; set; }
    bool YellowLine { get; set; }
    bool BlueLine { get; set; }
    string? YellowLineStatus { get; set; }
    string? BlueLineStatus { get; set; }
    bool TemporaryOperationEst { get; set; }
    bool PmRoutine { get; set; }
    bool CommissioningResp { get; set; }
    bool? ValveBlindingList { get; set; }
    DateTime LastUpdated { get; set; }
}
