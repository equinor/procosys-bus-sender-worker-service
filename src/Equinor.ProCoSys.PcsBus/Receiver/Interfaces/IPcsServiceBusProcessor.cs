using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;


namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

public interface IPcsServiceBusProcessor
{
    PcsTopic PcsTopic { get; }
    void RegisterPcsEventHandlers(Func<IPcsServiceBusProcessor, ProcessMessageEventArgs, Task> messageHandler, Func<ProcessErrorEventArgs, Task> errorHandler);

    Task StartProcessingAsync();

    Task StopProcessingAsync();
}
