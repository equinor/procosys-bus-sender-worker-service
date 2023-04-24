using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IQuerySignatureEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    string PlantName { get; set; }
    string ProjectName { get; set; }
    string? Status { get; set; }
    Guid? LibraryStatusGuid { get; set; }
    long QuerySignatureId { get; set; }
    long QueryId { get; set; }
    Guid QueryGuid { get; set; }
    string QueryNo { get; set; }
    string? SignatureRoleCode { get; set; }
    string? FunctionalRoleCode { get; set; }
    int Sequence { get; set; }
    Guid? SignedByAzureOid { get; set; }
    string? FunctionalRoleDescription { get; set; }
    DateTime? SignedDate { get; set; }
    DateTime LastUpdated { get; set; }
}
