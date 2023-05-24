using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver;

public class PcsServiceBusProcessors : IPcsServiceBusProcessors
{
    private readonly List<IPcsServiceBusProcessor> _serviceBusProcessors = new();


    public PcsServiceBusProcessors(int renewLeaseInterval)
    {
        if (renewLeaseInterval == 0)
        {
            throw new Exception("RenewLeaseIntervalMilliseconds must be a positive integer");
        }

        RenewLeaseInterval = renewLeaseInterval;
    }

    public void Add(IPcsServiceBusProcessor pcsServiceBusProcessor) =>
        _serviceBusProcessors.Add(pcsServiceBusProcessor);

    public async Task CloseAllAsync()
    {
        foreach (var s in _serviceBusProcessors)
        {
            await s.StopProcessingAsync();
        }
    }

    public void RegisterPcsEventHandlers(Func<IPcsServiceBusProcessor, ProcessMessageEventArgs, Task> messageHandler,
        Func<ProcessErrorEventArgs, Task> errorHandler) =>
        _serviceBusProcessors.ForEach(s =>
        {
            s.RegisterPcsEventHandlers(messageHandler, errorHandler);
        });

    public async void StartProcessingAsync()
    {
        foreach (var s in _serviceBusProcessors)
        {
            await s.StartProcessingAsync();
        }
    }

    public void UnRegisterPcsMessageHandler() =>
        _serviceBusProcessors.ForEach(s => s.StopProcessingAsync());

    public int RenewLeaseInterval { get; }
}
