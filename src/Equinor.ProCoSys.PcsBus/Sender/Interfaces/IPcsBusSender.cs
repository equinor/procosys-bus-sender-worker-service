using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;

public interface IPcsBusSender
{
    Task SendAsync(string topic, string jsonMessage);
    Task CloseAllAsync();
    ValueTask<ServiceBusMessageBatch> CreateMessageBatchAsync(string topic);
    Task SendMessagesAsync(ServiceBusMessageBatch messageBatch, string topic);
}
