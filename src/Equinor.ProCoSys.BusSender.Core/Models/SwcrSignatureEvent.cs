using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618
public class SwcrSignatureEvent : ISwcrSignatureEventV1
{
   
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public long SwcrSignatureId { get; set; }
    public string ProjectName { get; set; }
    public string SWCRNO { get; set; }
    public Guid SwcrGuid { get; set; }
    public string SignatureRoleCode { get; set; }
    public string? SignatureRoleDescription { get; set; }
    public int Sequence { get; set; }
    public Guid? SignedByAzureOid { get; set; }
    public string? FunctionalRoleCode { get; set; }
    public string? FunctionalRoleDescription { get; set; }
    public DateTime? SignedDate { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType  => PcsEventConstants.SwcrSignatureCreateOrUpdate;
}
