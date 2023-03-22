using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
namespace Equinor.ProCoSys.PcsServiceBus.Receiver;

public class PcsServiceBusProcessor : ServiceBusProcessor, IPcsServiceBusProcessor
{
    public string PcsTopic { get; }

    private Func<IPcsServiceBusProcessor, ProcessMessageEventArgs, Task?>?  _pcsHandler;

    public PcsServiceBusProcessor(ServiceBusClient client,string topicName, string subscriptionName,ServiceBusProcessorOptions options, string pcsTopic) 
        : base(client,topicName,subscriptionName,options) =>
        PcsTopic = pcsTopic;

    public void RegisterPcsEventHandlers(Func<IPcsServiceBusProcessor, ProcessMessageEventArgs, Task> handler,Func<ProcessErrorEventArgs,Task> errorHandler)
    {
        _pcsHandler = handler;
        ProcessMessageAsync += HandleMessage;
        ProcessErrorAsync += errorHandler;
    }

    public Task StopProcessingAsync() => base.StopProcessingAsync();

    public Task StartProcessingAsync() => base.StartProcessingAsync();

    private Task? HandleMessage(ProcessMessageEventArgs events) => _pcsHandler?.Invoke(this,events);
}
