﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;
using Microsoft.Azure.ServiceBus;

namespace Equinor.ProCoSys.PcsServiceBus.Sender
{
    public class PcsBusSender : IPcsBusSender
    {
        private readonly IList<KeyValuePair<string, ITopicClient>> _topicClients;

        public PcsBusSender() => _topicClients = new List<KeyValuePair<string, ITopicClient>>();

        public void Add(string topicName, ITopicClient topicClient) => _topicClients.Add(new KeyValuePair<string, ITopicClient>(topicName, topicClient));

        public Task SendAsync(string topic, string jsonMessage)
        {
            var message = new Message(Encoding.UTF8.GetBytes(jsonMessage));
            var topicClient = _topicClients.SingleOrDefault(t => t.Key == topic).Value;
            if (topicClient == null)
            {
                throw new Exception($"Unable to find TopicClient for topic: {topic}");
            }
            return topicClient.SendAsync(message);
        }

        public async Task CloseAllAsync()
        {
            foreach (var topicClient in _topicClients)
            {
                await topicClient.Value.CloseAsync();
            }
        }
    }
}
