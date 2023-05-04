using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ITagEventV1 : IHasEventType
{
    string Plant { get; }
    string PlantName { get; }
    Guid ProCoSysGuid { get; }
    long TagId { get; }
    string TagNo { get; }
    Guid? CommPkgGuid { get; }
    string? CommPkgNo { get; }
    Guid? McPkgGuid { get; }
    string? McPkgNo { get; }
    string? Description { get; }
    string? ProjectName { get; }
    string? AreaCode { get; }
    string? AreaDescription { get; }
    string? DisciplineCode { get; }
    string? DisciplineDescription { get; }
    string? RegisterCode { get; }
    string? InstallationCode { get; }
    string? Status { get; }
    string? System { get; }
    string? CallOffNo { get; }
    Guid? CallOffGuid { get; }
    string? PurchaseOrderNo { get; }
    string? TagFunctionCode { get; }
    string? EngineeringCode { get; }
    int? MountedOn { get; }
    Guid? MountedOnGuid { get; }
    bool IsVoided { get; }
    DateTime LastUpdated { get; }
    
}
