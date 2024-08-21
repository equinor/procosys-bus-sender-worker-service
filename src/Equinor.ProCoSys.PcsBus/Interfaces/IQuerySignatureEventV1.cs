using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface IQuerySignatureEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    string PlantName { get; init; }
    string ProjectName { get; init; }
    string? Status { get; init; }
    Guid? LibraryStatusGuid { get; init; }
    long QuerySignatureId { get; init; }
    long QueryId { get; init; }
    Guid QueryGuid { get; init; }
    string QueryNo { get; init; }
    string? SignatureRoleCode { get; init; }
    string? FunctionalRoleCode { get; init; }
    int Sequence { get; init; }
    Guid? SignedByAzureOid { get; init; }
    DateTime? SignedDate { get; init; }
    DateTime LastUpdated { get; init; }
}
