using System;
namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ICommPkgTaskEventV1
{
    string Plant { get; set; }
    string ProjectName { get; set; }
    Guid ProCoSysGuid { get; set; }
    Guid TaskGuid { get; set; }
    Guid CommPkgGuid { get; set; }
    string CommPkgNo { get; set; }
    DateTime LastUpdated { get; set; }
}
