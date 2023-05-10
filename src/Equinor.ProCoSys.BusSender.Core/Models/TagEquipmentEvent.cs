using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable 8618
public class TagEquipmentEvent : ITagEquipmentEventV1
{
    public string EventType => PcsEventConstants.TagEquipmentCreateOrUpdate;
    public string? EqHubId { get; init; }
    public DateTime LastUpdated { get; init; }
    public string ManufacturerName { get; init; }
    public string? ModelName { get; init; }
    public string? ModelNo { get; init; }
    public string? ModelSubName { get; init; }
    public string? ModelSubSubName { get; init; }
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public string? SemiId { get; init; }
    public Guid TagGuid { get; init; }
    public string TagNo { get; init; }
    public string? VariantNo { get; init; }
}
