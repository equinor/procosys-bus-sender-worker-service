using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class SwcrOtherReferenceEvent : ISwcrOtherReferenceEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public Guid LibraryGuid { get; set; }
    public Guid SwcrGuid { get; set; }
    public string Code { get; set; }
    public string? Description { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.SwcrOtherReferenceCreateOrUpdate;
}
