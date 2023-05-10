using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class CommPkgOperationEvent : ICommPkgOperationEventV1
{
    public bool BlueLine { get; init; }
    public string? BlueLineStatus { get; init; }
    public bool CommissioningResp { get; init; }
    public Guid CommPkgGuid { get; init; }
    public string CommPkgNo { get; init; }
    public bool InOperation { get; init; }
    public DateTime LastUpdated { get; init; }
    public bool MaintenanceProgram { get; init; }
    public string Plant { get; init; }
    public bool PmRoutine { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public bool ReadyForProduction { get; init; }
    public bool TemporaryOperationEst { get; init; }
    public bool? ValveBlindingList { get; init; }
    public bool YellowLine { get; init; }
    public string? YellowLineStatus { get; init; }

    public string EventType => PcsEventConstants.CommPkgOperationCreateOrUpdate;
}
