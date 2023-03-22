using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces.DeleteEvents;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

internal class ChecklistDeleteEvent : BaseDeleteEventV1
{
    public string Plant { get; }
    public Guid ProCoSysGuid { get; }

    public string EventType => PcsEventConstants.ChecklistDelete;
}
