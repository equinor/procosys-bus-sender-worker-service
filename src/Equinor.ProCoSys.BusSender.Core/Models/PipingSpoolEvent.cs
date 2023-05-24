using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable 8618

public class PipingSpoolEvent : IPipingSpoolEventV1
{
    public string EventType => PcsEventConstants.PipingSpoolCreateOrUpdate;
    public bool? AlternativeTest { get; init; }
    public int? AlternativeTestNoOfWelds { get; init; }
    public bool Installed { get; init; }
    public string ISODrawing { get; init; }
    public DateTime LastUpdated { get; init; }
    public Guid LineGuid { get; init; }
    public string LineNo { get; init; }
    public Guid McPkgGuid { get; init; }
    public string McPkgNo { get; init; }
    public bool? N2HeTest { get; init; }
    public bool? NDE { get; init; }
    public bool? Painted { get; init; }
    public Guid PipingRevisionGuid { get; init; }
    public int PipingRevisionId { get; init; }
    public int PipingSpoolId { get; init; }
    public string Plant { get; init; }
    public bool? PressureTested { get; init; }
    public bool? Primed { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public int Revision { get; init; }
    public string? Spool { get; init; }
    public bool? Welded { get; init; }
    public DateOnly? WeldedDate { get; init; }
}
