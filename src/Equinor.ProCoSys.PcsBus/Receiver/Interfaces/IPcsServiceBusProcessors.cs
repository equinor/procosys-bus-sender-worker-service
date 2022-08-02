using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;


namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

public interface IPcsServiceBusProcessors
{
    Task CloseAllAsync();
    void RegisterPcsMessageHandler(Func<ProcessMessageEventArgs, Task> handler);
    void UnRegisterPcsMessageHandler();
    int RenewLeaseInterval { get; }
}
