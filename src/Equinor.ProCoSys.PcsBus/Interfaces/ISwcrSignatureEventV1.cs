using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ISwcrSignatureEventV1 : IHasEventType
{
    string Plant { get; init; }
    Guid ProCoSysGuid { get; init; }
    long SwcrSignatureId { get; init; }
    string ProjectName { get; init; }
    string SwcrNo { get; init; }
    Guid SwcrGuid { get; init; }
    string SignatureRoleCode { get; init; }
    string? SignatureRoleDescription { get; init; }
    int Sequence { get; init; }
    Guid? SignedByAzureOid { get; init; }
    string? FunctionalRoleCode { get; init; }
    string? FunctionalRoleDescription { get; init; }
    DateTime? SignedDate { get; init; }
    DateTime LastUpdated { get; init; }
}
