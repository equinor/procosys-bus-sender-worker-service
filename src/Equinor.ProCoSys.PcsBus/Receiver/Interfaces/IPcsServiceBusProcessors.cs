using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;


namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

public interface IPcsServiceBusProcessors
{
    int RenewLeaseInterval { get; }
    Task CloseAllAsync();

    void RegisterPcsEventHandlers(Func<IPcsServiceBusProcessor, ProcessMessageEventArgs, Task> messageHandler,
        Func<ProcessErrorEventArgs, Task> errorHandler);

    void StartProcessingAsync();
    void UnRegisterPcsMessageHandler();
}
