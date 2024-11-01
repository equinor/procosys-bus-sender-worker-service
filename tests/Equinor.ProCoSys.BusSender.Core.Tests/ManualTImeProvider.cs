using System;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;
public class ManualTimeProvider : ISystemClock
{

    public DateTime UtcNow { get; private set; }

    public void Set(DateTime now)
    {
        UtcNow = now;
    }
}