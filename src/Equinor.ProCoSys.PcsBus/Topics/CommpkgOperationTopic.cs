namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class CommPkgOperationTopic
{
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public string CommPkgNo { get; set; }
    public bool InOperation { get; set; }
    public bool ReadyForProduction { get; set; }
    public bool MaintenanceProgram { get; set; }
    public bool YellowLine { get; set; }
    public bool BlueLine { get; set; }
    public string YellowLineStatus { get; set; }
    public string BlueLineStatus { get; set; }
    public bool TemporaryOperationEst { get; set; }
    public bool PmRoutine { get; set; }
    public bool CommissioningResp { get; set; }
    public string ValveBlindingList { get; set; }
    public string LastUpdated { get; set; }

    public const string TopicName = "commpkgoperation";

}
