using System;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
public interface ISystemClock
{
    DateTime UtcNow { get; }
}
