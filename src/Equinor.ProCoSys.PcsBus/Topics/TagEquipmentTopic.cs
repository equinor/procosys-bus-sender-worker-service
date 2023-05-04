using System;
#pragma warning disable CS8618

namespace Equinor.ProCoSys.PcsServiceBus.Topics;

public class TagEquipmentTopic
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string ManufacturerName { get; set; }
    public string ModelNo { get; set; }
    public string VariantNo { get; set; }
    public string EqHubId { get; set; }
    public string SemiId { get; set; }
    public string ModelName { get; set; }
    public string ModelSubName { get; set; }
    public string ModelSubSubName { get; set; }
    public Guid TagGuid { get; set; } 
    public string TagNo { get; set; } 
    public string ProjectNo { get; set; } 
    public string LastUpdated { get; set; }
    public string Behavior { get; set; }
    public const string TopicName = "tagequipment";
}
