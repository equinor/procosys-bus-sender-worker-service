using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;

namespace Equinor.ProCoSys.PcsServiceBus.Sender;

public class PcsBusSender : IPcsBusSender
{
    private readonly IList<KeyValuePair<string, ServiceBusSender>> _busSenders;

    public PcsBusSender() => _busSenders = new List<KeyValuePair<string, ServiceBusSender>>();

    public void Add(string topicName, ServiceBusSender sender) =>
        _busSenders.Add(new KeyValuePair<string, ServiceBusSender>(topicName, sender));

    public async Task CloseAllAsync()
    {
        foreach (var topicClient in _busSenders)
        {
            await topicClient.Value.CloseAsync();
        }
    }

    public async ValueTask<ServiceBusMessageBatch> CreateMessageBatchAsync(string topic)
    {
        var sender = _busSenders.SingleOrDefault(t => t.Key == topic).Value;
        if (sender == null)
        {
            throw new Exception($"Unable to find TopicClient for topic: {topic}");
        }

        var serviceBusMessageBatch = await sender.CreateMessageBatchAsync();
        return serviceBusMessageBatch;
    }

    public async Task SendAsync(string topic, string jsonMessage)
    {
        var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage));
        var sender = _busSenders.SingleOrDefault(t => t.Key == topic).Value;
        if (sender == null)
        {
            throw new Exception($"Unable to find TopicClient for topic: {topic}");
        }

        using var ts = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        await sender.SendMessageAsync(message);
        ts.Complete();
    }

    public async Task SendMessagesAsync(ServiceBusMessageBatch messageBatch, string topic)
    {
        var sender = _busSenders.SingleOrDefault(t => t.Key == topic).Value;
        if (sender == null)
        {
            throw new Exception($"Unable to find TopicClient for topic: {topic}");
        }

        await sender.SendMessagesAsync(messageBatch);
    }
}
