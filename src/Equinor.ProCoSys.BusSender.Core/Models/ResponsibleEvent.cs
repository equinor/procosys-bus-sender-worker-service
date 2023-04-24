using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class ResponsibleEvent : IResponsibleEventV1
{
    public string Plant { get; set; }
    public Guid ProCoSysGuid { get; set; }
    public long ResponsibleId { get; set; }
    public string Code { get; set; }
    public string ResponsibleGroup { get; set; }
    public string? Description { get; set; }
    public bool IsVoided { get; set; }
    public DateTime LastUpdated { get; set; }
    
    public string EventType => PcsEventConstants.ResponsibleCreateOrUpdate;
}
