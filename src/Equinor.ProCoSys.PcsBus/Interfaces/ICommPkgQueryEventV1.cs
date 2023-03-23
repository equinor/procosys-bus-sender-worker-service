using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ICommPkgQueryEventV1 : IHasEventType
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public long CommPkgId { get; set; }
    public Guid CommPkgGuid { get; set; }
    public string CommPkgNo { get; set; }
    public long DocumentId { get; set; }
    public string QueryNo { get; set; }
    public Guid QueryGuid { get; set; }
    public DateTime LastUpdated { get; set; }
}
