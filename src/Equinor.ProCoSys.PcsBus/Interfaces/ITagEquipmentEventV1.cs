using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ITagEquipmentEventV1 : IHasEventType
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
}
