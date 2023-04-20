using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class LibraryEvent : ILibraryEventV1 
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public long LibraryId { get; set; }
    public int? ParentId { get; set; }
    public Guid? ParentGuid { get; set; }
    public string Code { get; set; }
    public string? Description { get; set; }
    public bool IsVoided { get; set; }
    public string Type { get; set; }
    public DateTime LastUpdated { get; set; }
    public string EventType => PcsEventConstants.LibraryCreateOrUpdate;
}
