﻿using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IQueueMonitorService
{
    Task WriteQueueMetrics(string? plant = null);
}
