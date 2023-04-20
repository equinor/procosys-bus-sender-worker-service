using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IPipingSpoolEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string ProjectName { get; set; }
    int PipingSpoolId { get; set; }
    int PipingRevisionId { get; set; }
    Guid PipingRevisionGuid { get; set; }
    int Revision { get; set; }
    string McPkgNo { get; set; }
    Guid McPkgGuid { get; set; }
    string ISODrawing { get; set; }
    string? Spool { get; set; }
    string LineNo { get; set; }
    Guid LineGuid { get; set; }
    bool? N2HeTest { get; set; }
    bool? AlternativeTest { get; set; }
    int? AlternativeTestNoOfWelds { get; set; }
    bool Installed { get; set; }
    bool? Welded { get; set; }
    DateOnly? WeldedDate { get; set; }
    bool? PressureTested { get; set; }
    bool? NDE { get; set; }
    bool? Primed { get; set; }
    bool? Painted { get; set; }
    DateTime LastUpdated { get; set; }
}
