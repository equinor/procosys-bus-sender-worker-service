using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces.DeleteEvents;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;
#pragma warning disable CS8618

//Not in use yet
public class ChecklistDeleteEvent : IChecklistDeleteEventV1
{
    public string Plant { get; init; }
    public Guid ProCoSysGuid { get; init; }

    public string EventType => PcsEventConstants.ChecklistDelete;
}


