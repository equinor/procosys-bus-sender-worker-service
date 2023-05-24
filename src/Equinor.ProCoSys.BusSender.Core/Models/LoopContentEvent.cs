using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class LoopContentEvent : ILoopContentEventV1
{
    public string EventType => PcsEventConstants.LoopTagCreateOrUpdate;
    public DateTime LastUpdated { get; init; }
    public Guid LoopTagGuid { get; init; }
    public int LoopTagId { get; init; }
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string RegisterCode { get; init; }
    public Guid TagGuid { get; init; }
    public int TagId { get; init; }
}
