using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

public interface IPcsServiceBusProcessor
{
    void RegisterPcsEventHandlers(Func<IPcsServiceBusProcessor, ProcessMessageEventArgs, Task> messageHandler,
        Func<ProcessErrorEventArgs, Task> errorHandler);

    Task StartProcessingAsync();

    Task StopProcessingAsync();
    string PcsTopic { get; }
}
