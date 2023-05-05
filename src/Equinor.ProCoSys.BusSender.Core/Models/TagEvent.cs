using System;
using System.Collections.Generic;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class TagEvent : ITagEventV1
{
    
    public string Plant { get; }
    public string PlantName { get; }
    public Guid ProCoSysGuid { get; }
    public long TagId { get; }
    public string TagNo { get; }
    public Guid? CommPkgGuid { get; }
    public string? CommPkgNo { get; }
    public Guid? McPkgGuid { get; }
    public string? McPkgNo { get; }
    public string? Description { get; }
    public string? ProjectName { get; }
    public string? AreaCode { get; }
    public string? AreaDescription { get; }
    public string? DisciplineCode { get; }
    public string? DisciplineDescription { get; }
    public string? RegisterCode { get; }
    public string? InstallationCode { get; }
    public string? Status { get; }
    public string? System { get; }
    public string? CallOffNo { get; }
    public Guid? CallOffGuid { get; }
    public string? PurchaseOrderNo { get; }
    public string? TagFunctionCode { get; }
    public string? EngineeringCode { get; }
    public int? MountedOn { get; }
    public Guid? MountedOnGuid { get; }
    public bool IsVoided { get; }
    public DateTime LastUpdated { get; }
    
    public List<dynamic>? TagDetails { get; set; }
    
    public string EventType => PcsEventConstants.TagCreateOrUpdate;
}
