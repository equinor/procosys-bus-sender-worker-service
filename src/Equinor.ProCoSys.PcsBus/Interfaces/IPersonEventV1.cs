using System;
using JetBrains.Annotations;

namespace Equinor.ProCoSys.PcsServiceBus.Interfaces;

[UsedImplicitly]
public interface IPersonEventV1 : IHasEventType
{
    Guid Guid { get; init; }
    string FirstName { get; init; }
    string LastName { get; init; }
    string UserName { get; init; }
    string Email { get; init; }
    bool SuperUser { get; init; }
    DateTime LastUpdated { get; init; }

}
