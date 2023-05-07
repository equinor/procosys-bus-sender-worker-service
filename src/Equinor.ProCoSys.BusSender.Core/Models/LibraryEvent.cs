using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
#pragma warning disable CS8618
namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public class LibraryEvent : ILibraryEventV1 
{
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public long LibraryId { get; init; }
    public int? ParentId { get; init; }
    public Guid? ParentGuid { get; init; }
    public string Code { get; init; }
    public string? Description { get; init; }
    public bool IsVoided { get; init; }
    public string Type { get; init; }
    public DateTime LastUpdated { get; init; }
    public string EventType => PcsEventConstants.LibraryCreateOrUpdate;
}
