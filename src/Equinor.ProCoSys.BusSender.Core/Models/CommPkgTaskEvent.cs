using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class CommPkgTaskEvent : ICommPkgTaskEventV1
{
    public string Plant { get; set; }
    public string ProjectName { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public Guid TaskGuid { get; set; }
    public Guid CommPkgGuid { get; set; }
    public string CommPkgNo { get; set; }
    public DateTime LastUpdated { get; set; }
    public string EventType => PcsEventConstants.CommPkgTaskCreateOrUpdate;
}
