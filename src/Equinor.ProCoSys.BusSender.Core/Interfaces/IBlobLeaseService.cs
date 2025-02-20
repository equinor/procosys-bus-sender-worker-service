﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
public interface IBlobLeaseService
{
    void ReleasePlantLease(PlantLease? plantLease);
    Task<List<PlantLease>?> ClaimPlantLease();
    CancellationToken CancellationToken { get; }
    IMemoryCache GetCache();
}
