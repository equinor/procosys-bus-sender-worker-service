using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ILoopContentEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    int LoopTagId { get; set; }
    Guid LoopTagGuid { get; set; }
    int TagId { get; set; }
    Guid TagGuid { get; set; }
    string RegisterCode { get; set; }
    DateTime LastUpdated { get; set; }
}

