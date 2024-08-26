using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ITagEventV1 : IHasEventType
{
    string Plant { get; init; }
    string PlantName { get; init; }
    Guid ProCoSysGuid { get; init; }
    long TagId { get; init; }
    string TagNo { get; init; }
    Guid? CommPkgGuid { get; init; }
    string? CommPkgNo { get; init; }
    Guid? McPkgGuid { get; init; }
    string? McPkgNo { get; init; }
    string? Description { get; init; }
    string? ProjectName { get; init; }
    string? AreaCode { get; init; }
    string? AreaDescription { get; init; }
    string? DisciplineCode { get; init; }
    string? DisciplineDescription { get; init; }
    string? RegisterCode { get; init; }
    string? InstallationCode { get; init; }
    string? Status { get; init; }
    string? System { get; init; }
    string? CallOffNo { get; init; }
    Guid? CallOffGuid { get; init; }
    string? PurchaseOrderNo { get; init; }
    string? TagFunctionCode { get; init; }
    string? EngineeringCode { get; init; }
    string? ContractorCode { get; init; }
    int? MountedOn { get; init; }
    Guid? MountedOnGuid { get; init; }
    bool IsVoided { get; init; }
    DateTime LastUpdated { get; init; }

    string? TagDetails { get; init; }
}
