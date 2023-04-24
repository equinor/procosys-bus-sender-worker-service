using System;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

public interface ISwcrSignatureEventV1 : IHasEventType
{
    string Plant { get; set; }
    Guid ProCoSysGuid { get; set; }
    long SwcrSignatureId { get; set; }
    string ProjectName { get; set; }
    string SWCRNO { get; set; }
    Guid SwcrGuid { get; set; }
    string SignatureRoleCode { get; set; }
    string? SignatureRoleDescription { get; set; }
    int Sequence { get; set; }
    Guid? SignedByAzureOid { get; set; }
    string? FunctionalRoleCode { get; set; }
    string? FunctionalRoleDescription { get; set; }
    DateTime? SignedDate { get; set; }
    DateTime LastUpdated { get; set; }
}
