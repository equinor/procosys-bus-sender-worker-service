using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver;

public class PcsServiceBusProcessors : IPcsServiceBusProcessors
{
    private readonly List<ServiceBusProcessor> _serviceBusProcessors = new();

    public PcsServiceBusProcessors(int renewLeaseInterval)
    {
        if (renewLeaseInterval == 0)
        {
            throw new Exception("RenewLeaseIntervalMilliSec must be a positive integer");
        }

        RenewLeaseInterval = renewLeaseInterval;
    }

    public void Add(ServiceBusProcessor pcsServiceBusProcessor) => _serviceBusProcessors.Add(pcsServiceBusProcessor);

    public async Task CloseAllAsync()
    {
        foreach (var s in _serviceBusProcessors)
        {
            await s.CloseAsync();
        }
    }

    public void RegisterPcsMessageHandler(
        Func<ProcessMessageEventArgs, Task> handler) =>
        _serviceBusProcessors.ForEach(s =>
        {
            s.ProcessMessageAsync += handler;
        });

    public void UnRegisterPcsMessageHandler() =>
        _serviceBusProcessors.ForEach(s => s.ProcessMessageAsync += args => default);

    public int RenewLeaseInterval { get; }
}
