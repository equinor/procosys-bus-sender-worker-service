using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;


namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

public interface IPcsServiceBusProcessor
{
    PcsTopic PcsTopic { get; }
    void RegisterPcsMessageHandler(Func<IPcsServiceBusProcessor, ProcessMessageEventArgs, Task> handler);

    Task StartProcessingAsync();

    Task StopProcessingAsync();
}
