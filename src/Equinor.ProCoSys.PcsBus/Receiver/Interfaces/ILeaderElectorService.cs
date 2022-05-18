using System;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

public interface ILeaderElectorService
{
    Task<bool> CanProceedAsLeader(Guid id);
}
