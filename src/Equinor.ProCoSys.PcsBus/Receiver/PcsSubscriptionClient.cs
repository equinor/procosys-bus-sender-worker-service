using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver
{
    public class PcsSubscriptionClient : IPcsSubscriptionClient
    {
        private ServiceBusReceiver _client;

        public PcsTopic PcsTopic { get; }

        private Func<IPcsSubscriptionClient, ServiceBusReceivedMessage, CancellationToken, Task> _pcsHandler;
        public PcsSubscriptionClient(ServiceBusClient client, PcsTopic pcsTopic, string subscriptionName)
        {
            var options = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false
            };

            _client  = client.CreateReceiver(subscriptionName, options);
            PcsTopic = pcsTopic;
        }

        public void RegisterPcsMessageHandler(Func<IPcsSubscriptionClient, ServiceBusReceivedMessage, CancellationToken, Task> handler, ServiceBusProcessorOptions messageHandlerOptions)
        {
            _pcsHandler = handler;
            _client.ProcessMessageAsync += HandleMessage;
        }

        public void UnregisterPcsMessageHandler() => base.UnregisterMessageHandlerAsync(TimeSpan.FromSeconds(10));

        private Task HandleMessage(ProcessMessageEventArgs args) => _pcsHandler.Invoke(this, args);
    }
}
