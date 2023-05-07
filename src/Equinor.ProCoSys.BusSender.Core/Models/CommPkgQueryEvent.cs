using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class CommPkgQueryEvent : ICommPkgQueryEventV1
{
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public long CommPkgId { get; init; }
    public Guid CommPkgGuid { get; init; }
    public string CommPkgNo { get; init; }
    public long DocumentId { get; init; }
    public string QueryNo { get; init; }
    public Guid QueryGuid { get; init; }
    public DateTime LastUpdated { get; init; }
    
    public string EventType => PcsEventConstants.CommPkgQueryCreateOrUpdate;
}
