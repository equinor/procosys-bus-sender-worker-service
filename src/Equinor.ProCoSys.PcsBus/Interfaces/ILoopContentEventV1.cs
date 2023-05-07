using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ILoopContentEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    int LoopTagId { get; init; }
    Guid LoopTagGuid { get; init; }
    int TagId { get; init; }
    Guid TagGuid { get; init; }
    string RegisterCode { get; init; }
    DateTime LastUpdated { get; init; }
}

