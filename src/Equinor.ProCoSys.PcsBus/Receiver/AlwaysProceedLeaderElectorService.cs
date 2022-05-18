using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver;

public class AlwaysProceedLeaderElectorService : ILeaderElectorService
{
    public Task<bool> CanProceedAsLeader(Guid id) => Task.FromResult(true);
}
