using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class SwcrSignatureEvent : ISwcrSignatureEventV1
{
    public string EventType => PcsEventConstants.SwcrSignatureCreateOrUpdate;
    public string? FunctionalRoleCode { get; init; }
    public string? FunctionalRoleDescription { get; init; }
    
    public string? StatusCode { get; init; }
    public DateTime LastUpdated { get; init; }

    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ProjectName { get; init; }
    public int Sequence { get; init; }
    public string SignatureRoleCode { get; init; }
    public string? SignatureRoleDescription { get; init; }
    public Guid? SignedByAzureOid { get; init; }
    public DateTime? SignedDate { get; init; }
    public Guid SwcrGuid { get; init; }
    public string SwcrNo { get; init; }
    public long SwcrSignatureId { get; init; }
}
