using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ITagDocumentEventV1 : IHasEventType
{
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public long TagId { get; init; }
    public Guid TagGuid { get; init; }
    public long DocumentId { get; init; }
    public Guid DocumentGuid { get; init; }
    public DateTime LastUpdated { get; init; }
}
