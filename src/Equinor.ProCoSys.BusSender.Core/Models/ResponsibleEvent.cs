using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

public class ResponsibleEvent : IResponsibleEventV1
{
    public string EventType => PcsEventConstants.ResponsibleCreateOrUpdate;
    public string Code { get; init; }
    public string? Description { get; init; }
    public bool IsVoided { get; init; }
    public DateTime LastUpdated { get; init; }
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }
    public string ResponsibleGroup { get; init; }
    public long ResponsibleId { get; init; }
}
