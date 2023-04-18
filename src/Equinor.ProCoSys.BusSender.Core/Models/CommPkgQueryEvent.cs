using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class CommPkgQueryEvent : ICommPkgQueryEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public long CommPkgId { get; set; }
    public Guid CommPkgGuid { get; set; }
    public string CommPkgNo { get; set; }
    public long DocumentId { get; set; }
    public string QueryNo { get; set; }
    public Guid QueryGuid { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.CommPkgQueryCreateOrUpdate;
}
