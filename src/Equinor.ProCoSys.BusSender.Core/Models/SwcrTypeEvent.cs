using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class SwcrTypeEvent : ISwcrTypeEventV1
{
    public string EventType => PcsEventConstants.SwcrTypeCreateOrUpdate;
    public string Code { get; init; }
    public DateTime LastUpdated { get; init; }
    public Guid LibraryGuid { get; init; }

    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public Guid SwcrGuid { get; init; }
}
