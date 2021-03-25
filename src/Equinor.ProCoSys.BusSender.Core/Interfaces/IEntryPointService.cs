﻿using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces
{
    public interface IEntryPointService
    {
        Task SendMessageChunk();
        Task StopService();
    }
}
