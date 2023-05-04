using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable 8618
public class TagEquipmentEvent : ITagEquipmentEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ManufacturerName { get; set; }
    public string? ModelNo { get; set; }
    public string? VariantNo { get; set; }
    public string? EqHubId { get; set; }
    public string? SemiId { get; set; }
    public string? ModelName { get; set; }
    public string? ModelSubName { get; set; }
    public string? ModelSubSubName { get; set; }
    public Guid TagGuid { get; set; }
    public string TagNo { get; set; }
    public string ProjectName { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.TagEquipmentCreateOrUpdate;
}
