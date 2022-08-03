using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
namespace Equinor.ProCoSys.PcsServiceBus.Receiver;

public class PcsServiceBusProcessor : ServiceBusProcessor, IPcsServiceBusProcessor
{
    public PcsTopic PcsTopic { get; }

    private Func<IPcsServiceBusProcessor,ProcessMessageEventArgs, Task>  _pcsHandler;

    public PcsServiceBusProcessor(ServiceBusClient client,string topicName, string subscriptionName,ServiceBusProcessorOptions options, PcsTopic pcsTopic) 
        : base(client,topicName,subscriptionName,options) =>
        PcsTopic = pcsTopic;

    public void RegisterPcsMessageHandler(Func<IPcsServiceBusProcessor, ProcessMessageEventArgs, Task> handler)
    {
        _pcsHandler = handler;
        ProcessMessageAsync += HandleMessage;
    }

    public Task StopProcessingAsync() => base.StopProcessingAsync();

    public Task StartProcessingAsync() => base.StartProcessingAsync();

    private Task HandleMessage(ProcessMessageEventArgs events) => _pcsHandler.Invoke(this,events);
}
