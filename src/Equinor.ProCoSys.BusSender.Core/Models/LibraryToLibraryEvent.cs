using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public sealed record LibraryToLibraryEvent(
    string Plant,
    Guid ProCoSysGuid,
    string Role,
    string Association,
    Guid LibraryGuid,
    Guid RelatedLibraryGuid,
    DateTime LastUpdated
) : ILibraryToLibraryEventV1
{
    public string EventType => PcsEventConstants.LibraryToLibraryCreateOrUpdate;
}


