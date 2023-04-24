using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class LoopContentEvent : ILoopContentEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public int LoopTagId { get; set; }
    public Guid LoopTagGuid { get; set; }
    public int TagId { get; set; }
    public Guid TagGuid { get; set; }
    public string RegisterCode { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.LoopTagCreateOrUpdate;
}
