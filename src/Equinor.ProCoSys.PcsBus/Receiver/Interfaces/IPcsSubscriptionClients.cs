﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces
{
    public interface IPcsSubscriptionClients
    {
        Task CloseAllAsync();
        void RegisterPcsMessageHandler(Func<IPcsSubscriptionClient, Message, CancellationToken, Task> handler, MessageHandlerOptions options);
    }
}
