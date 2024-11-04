using System;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class TimeService : ISystemClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}
