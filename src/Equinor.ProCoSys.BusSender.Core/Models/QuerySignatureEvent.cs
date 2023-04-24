using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class QuerySignatureEvent : IQuerySignatureEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public string PlantName { get; set; }
    public string ProjectName { get; set; }
    public string? Status { get; set; }
    public Guid? LibraryStatusGuid { get; set; }
    public long QuerySignatureId { get; set; }
    public long QueryId { get; set; }
    public Guid QueryGuid { get; set; }
    public string QueryNo { get; set; }
    public string? SignatureRoleCode { get; set; }
    public string? FunctionalRoleCode { get; set; }
    public int Sequence { get; set; }
    public Guid? SignedByAzureOid { get; set; }
    public string? FunctionalRoleDescription { get; set; }
    public DateTime? SignedDate { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.QuerySignatureCreateOrUpdate;
}
