using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
namespace Equinor.ProCoSys.PcsServiceBus.Receiver;

public class PcsServiceBusProcessor : ServiceBusProcessor, IPcsServiceBusProcessor
{
    public PcsTopic PcsTopic { get; }

    private Func<IPcsServiceBusProcessor, ServiceBusMessage, CancellationToken, Task> _pcsHandler;


    public void RegisterPcsMessageHandler(Func<IPcsServiceBusProcessor, ServiceBusMessage, CancellationToken, Task> handler, ServiceBusProcessorOptions messageHandlerOptions)
    {
        _pcsHandler = handler;
        base.ProcessMessageAsync(HandleMessage, messageHandlerOptions);
    }

    public Task CompleteAsync(string token) => throw new NotImplementedException();

    public Task CloseAsync() => throw new NotImplementedException();

    public void UnRegisterPcsMessageHandler() => base.UnregisterMessageHandlerAsync(TimeSpan.FromSeconds(10));

    private Task HandleMessage(ServiceBusMessage message, CancellationToken token) => _pcsHandler.Invoke(this, message, token);
}
