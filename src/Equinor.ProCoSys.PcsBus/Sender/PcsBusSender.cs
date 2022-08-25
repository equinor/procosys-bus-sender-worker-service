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
    private readonly ServiceBusClient _serviceBusClient;

    public PcsBusSender(string connectionString, List<string> topics)
    {
        _busSenders = new List<KeyValuePair<string, ServiceBusSender>>();
        var options = new ServiceBusClientOptions { EnableCrossEntityTransactions = true };
        _serviceBusClient = new ServiceBusClient(connectionString, options);
        topics.ForEach(AddServiceBusSender);
    }

    public void AddServiceBusSender(string topicName)
    {
        var serviceBusSender = _serviceBusClient.CreateSender(topicName);
        _busSenders.Add(new KeyValuePair<string, ServiceBusSender>(topicName, serviceBusSender));
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

    public async Task CloseAllAsync()
    {
        foreach (var senders in _busSenders)
        {
            await senders.Value.CloseAsync();
        }
        await _serviceBusClient.DisposeAsync();
    }
}
