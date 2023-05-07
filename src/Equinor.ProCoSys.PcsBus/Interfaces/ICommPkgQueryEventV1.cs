using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface ICommPkgQueryEventV1 : IHasEventType
{
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public long CommPkgId { get; init; }
    public Guid CommPkgGuid { get; init; }
    public string CommPkgNo { get; init; }
    public long DocumentId { get; init; }
    public string QueryNo { get; init; }
    public Guid QueryGuid { get; init; }
    public DateTime LastUpdated { get; init; }
}
