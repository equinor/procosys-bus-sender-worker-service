using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class CommPkgTaskEvent : ICommPkgTaskEventV1
{
    public Guid CommPkgGuid { get; init; }
    public string CommPkgNo { get; init; }
    public DateTime LastUpdated { get; init; }
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public Guid TaskGuid { get; init; }
    public string EventType => PcsEventConstants.CommPkgTaskCreateOrUpdate;
}
