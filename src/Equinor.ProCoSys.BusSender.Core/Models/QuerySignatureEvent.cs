using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class QuerySignatureEvent : IQuerySignatureEventV1
{
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string PlantName { get; init; }
    public string ProjectName { get; init; }
    public string? Status { get; init; }
    public Guid? LibraryStatusGuid { get; init; }
    public long QuerySignatureId { get; init; }
    public long QueryId { get; init; }
    public Guid QueryGuid { get; init; }
    public string QueryNo { get; init; }
    public string? SignatureRoleCode { get; init; }
    public string? FunctionalRoleCode { get; init; }
    public int Sequence { get; init; }
    public Guid? SignedByAzureOid { get; init; }
    public string? FunctionalRoleDescription { get; init; }
    public DateTime? SignedDate { get; init; }
    public DateTime LastUpdated { get; init; }
    
    public string EventType => PcsEventConstants.QuerySignatureCreateOrUpdate;
}
