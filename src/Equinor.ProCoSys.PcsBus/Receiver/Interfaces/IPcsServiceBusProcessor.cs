using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;


namespace Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

public interface IPcsServiceBusProcessor
{
    PcsTopic PcsTopic { get; }
    void RegisterPcsMessageHandler(Func<IPcsServiceBusProcessor, ServiceBusMessage, CancellationToken, Task> handler, ServiceBusProcessorOptions processorOptions);
    Task CompleteAsync(string token);
    Task CloseAsync();
    void UnRegisterPcsMessageHandler();
}
