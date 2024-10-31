using Equinor.ProCoSys.Common.Time;
using System;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;
public class ManualTimeProvider : ITimeProvider
{
    public ManualTimeProvider(DateTime now)
    {
        if (now.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Must be UTC");
        }

        UtcNow = now;
    }

    public DateTime UtcNow { get; private set; }
}