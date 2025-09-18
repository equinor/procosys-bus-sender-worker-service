using System;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Models;

public sealed record LibraryToLibraryEvent(
    Guid ProCoSysGuid,
    string Plant,
    string Role,
    string Association,
    Guid LibraryGuid,
    Guid RelatedLibraryGuid,
    DateTime LastUpdated,
    string EventType = PcsEventConstants.LibraryToLibraryCreateOrUpdate
) : ILibraryToLibraryEventV1;


