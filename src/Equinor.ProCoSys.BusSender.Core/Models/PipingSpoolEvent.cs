using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable 8618

public class PipingSpoolEvent : IPipingSpoolEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ProjectName { get; set; }
    public int PipingSpoolId { get; set; }
    public int PipingRevisionId { get; set; }
    public Guid PipingRevisionGuid { get; set; }
    public int Revision { get; set; }
    public string McPkgNo { get; set; }
    public Guid McPkgGuid { get; set; }
    public string ISODrawing { get; set; }
    public string? Spool { get; set; }
    public string LineNo { get; set; }
    public Guid LineGuid { get; set; }
    public bool? N2HeTest { get; set; }
    public bool? AlternativeTest { get; set; }
    public int? AlternativeTestNoOfWelds { get; set; }
    public bool Installed { get; set; }
    public bool? Welded { get; set; }
    public DateOnly? WeldedDate { get; set; }
    public bool? PressureTested { get; set; }
    public bool? NDE { get; set; }
    public bool? Primed { get; set; }
    public bool? Painted { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.PipingSpoolCreateOrUpdate;
}
