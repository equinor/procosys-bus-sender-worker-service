using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;

public interface IPcsBusSender
{
    Task CloseAllAsync();
    ValueTask<ServiceBusMessageBatch> CreateMessageBatchAsync(string topic);
    Task SendAsync(string topic, string jsonMessage);
    Task SendMessagesAsync(ServiceBusMessageBatch messageBatch, string topic);
}
