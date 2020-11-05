using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Core.Interfaces;
using Microsoft.Azure.ServiceBus;

namespace Equinor.ProCoSys.BusSender.Core
{
    public class TopicClients : ITopicClients
    {
        private readonly IList<KeyValuePair<string, ITopicClient>> _topicClients;

        public TopicClients() => _topicClients = new List<KeyValuePair<string, ITopicClient>>();

        public void Add(string topicName, ITopicClient topicClient) => _topicClients.Add(new KeyValuePair<string, ITopicClient>(topicName, topicClient));

        public Task Send(string topic, string message)
        {
            var topicClient = _topicClients.SingleOrDefault(t => t.Key == topic).Value;
            if (topicClient == null)
            {
                throw new Exception($"Unable to find TopicClient for topic: {topic}");
            }
            return topicClient.SendAsync(new Message(Encoding.UTF8.GetBytes(message)));
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
