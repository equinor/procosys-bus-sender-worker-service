using System.Threading;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

public interface IBusReceiverService
{
    Task ProcessMessageAsync(PcsTopic pcsTopic, string message, CancellationToken token);
}
