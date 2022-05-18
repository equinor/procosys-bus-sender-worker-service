using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

public interface IBusReceiverService
{
    Task ProcessMessageAsync(PcsTopic pcsTopic, string message, CancellationToken token);
}
