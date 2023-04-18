using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class CommPkgOperationEvent :ICommPkgOperationEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public string CommPkgNo { get; set; }
    public Guid CommPkgGuid { get; set; }
    public bool InOperation { get; set; }
    public bool ReadyForProduction { get; set; }
    public bool MaintenanceProgram { get; set; }
    public bool YellowLine { get; set; }
    public bool BlueLine { get; set; }
    public string? YellowLineStatus { get; set; }
    public string? BlueLineStatus { get; set; }
    public bool TemporaryOperationEst { get; set; }
    public bool PmRoutine { get; set; }
    public bool CommissioningResp { get; set; }
    public bool? ValveBlindingList { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.CommPkgOperationCreateOrUpdate;
}
