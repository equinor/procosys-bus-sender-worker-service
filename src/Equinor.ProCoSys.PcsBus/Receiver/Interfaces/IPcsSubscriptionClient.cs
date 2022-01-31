using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces
{
    public interface IPcsSubscriptionClient
    {
        PcsTopic PcsTopic { get; }
        void RegisterPcsMessageHandler(Func<IPcsSubscriptionClient, ServiceBusReceivedMessage, CancellationToken, Task> handler, MessageHandlerOptions messageHandlerOptions);
        Task CloseAsync();
        void UnregisterPcsMessageHandler();
    }
}
