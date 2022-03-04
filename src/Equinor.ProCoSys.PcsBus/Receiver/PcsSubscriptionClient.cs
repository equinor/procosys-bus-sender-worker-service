using System;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Microsoft.Azure.ServiceBus;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver
{
    public class PcsSubscriptionClient : SubscriptionClient, IPcsSubscriptionClient
    {
        public PcsTopic PcsTopic { get; }

        private Func<IPcsSubscriptionClient, Message, CancellationToken, Task> _pcsHandler;

        public PcsSubscriptionClient(string connectionString, PcsTopic pcsTopic, string topicPath, string subscriptionName )
            : base(connectionString, string.IsNullOrWhiteSpace(topicPath)? pcsTopic.ToString() :topicPath, subscriptionName, ReceiveMode.PeekLock, RetryPolicy.Default) =>
            PcsTopic = pcsTopic;

        public void RegisterPcsMessageHandler(Func<IPcsSubscriptionClient, Message, CancellationToken, Task> handler, MessageHandlerOptions messageHandlerOptions)
        {
            _pcsHandler = handler;
            base.RegisterMessageHandler(HandleMessage, messageHandlerOptions);
        }

        public void UnregisterPcsMessageHandler() => base.UnregisterMessageHandlerAsync(TimeSpan.FromSeconds(10));

        private Task HandleMessage(Message message, CancellationToken token) => _pcsHandler.Invoke(this, message, token);
    }
}
