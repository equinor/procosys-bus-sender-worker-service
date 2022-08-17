using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;


namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

public interface IPcsServiceBusProcessors
{
    Task CloseAllAsync();
    void RegisterPcsEventHandlers(Func<IPcsServiceBusProcessor,ProcessMessageEventArgs, Task> messageHandler, Func<ProcessErrorEventArgs, Task> errorHandler );
    void UnRegisterPcsMessageHandler();
    int RenewLeaseInterval { get; }
    void StartProcessingAsync();
}
