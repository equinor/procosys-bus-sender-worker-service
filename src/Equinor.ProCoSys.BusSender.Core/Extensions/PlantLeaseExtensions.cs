using System;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Extensions;
public static class PlantLeaseExtensions
{
    public static bool IsExpired(this PlantLease plantLease) => plantLease.LeaseExpiry.HasValue && DateTime.UtcNow >= plantLease.LeaseExpiry.Value;
}
