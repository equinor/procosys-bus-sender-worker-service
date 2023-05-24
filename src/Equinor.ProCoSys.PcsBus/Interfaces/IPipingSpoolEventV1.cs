using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IPipingSpoolEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string ProjectName { get; init; }
    int PipingSpoolId { get; init; }
    int PipingRevisionId { get; init; }
    Guid PipingRevisionGuid { get; init; }
    int Revision { get; init; }
    string McPkgNo { get; init; }
    Guid McPkgGuid { get; init; }
    string ISODrawing { get; init; }
    string? Spool { get; init; }
    string LineNo { get; init; }
    Guid LineGuid { get; init; }
    bool? N2HeTest { get; init; }
    bool? AlternativeTest { get; init; }
    int? AlternativeTestNoOfWelds { get; init; }
    bool Installed { get; init; }
    bool? Welded { get; init; }
    DateOnly? WeldedDate { get; init; }
    bool? PressureTested { get; init; }
    bool? NDE { get; init; }
    bool? Primed { get; init; }
    bool? Painted { get; init; }
    DateTime LastUpdated { get; init; }
}
