using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces
{
    public interface IPcsSubscriptionClients
    {
        Task CloseAllAsync();
        void RegisterPcsMessageHandler(Func<IPcsSubscriptionClient, ProcessMessageEventArgs, CancellationToken, Task> handler, MessageHandlerOptions options);
        void UnregisterPcsMessageHandler();
        int RenewLeaseInterval { get; }
    }
}
