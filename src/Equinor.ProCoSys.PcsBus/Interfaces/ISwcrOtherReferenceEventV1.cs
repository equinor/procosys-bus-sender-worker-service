using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ISwcrOtherReferenceEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    Guid LibraryGuid { get; set; }
    Guid SwcrGuid { get; set; }
    string Code { get; set; }
    string? Description { get; set; }
    DateTime LastUpdated { get; set; } 
}
