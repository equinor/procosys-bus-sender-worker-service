using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Microsoft.Azure.ServiceBus;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver
{
    public class PcsSubscriptionClients : IPcsSubscriptionClients
    {
        private readonly List<IPcsSubscriptionClient> _subscriptionClients = new();

        public PcsSubscriptionClients(int renewLeaseInterval)
        {
            if (renewLeaseInterval == 0)
            {
                throw new Exception("RenewLeaseInterval must be a positive integer");
            }

            RenewLeaseInterval = renewLeaseInterval;
        }

        public void Add(IPcsSubscriptionClient pcsSubscriptionClient) => _subscriptionClients.Add(pcsSubscriptionClient);

        public async Task CloseAllAsync()
        {
            foreach (var s in _subscriptionClients)
            {
                await s.CloseAsync();
            }
        }

        public void RegisterPcsMessageHandler(
            Func<IPcsSubscriptionClient, Message, CancellationToken, Task> handler,
            MessageHandlerOptions options) =>
                _subscriptionClients.ForEach(s => s.RegisterPcsMessageHandler(handler, options));

        public void UnregisterPcsMessageHandler() =>
                _subscriptionClients.ForEach(s => s.UnregisterPcsMessageHandler());

        public int RenewLeaseInterval { get; }
    }
}
