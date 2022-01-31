using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;

namespace Equinor.ProCoSys.PcsServiceBus.Sender
{
    public class PcsBusSender : IPcsBusSender
    {
        private readonly IList<KeyValuePair<string, ServiceBusSender>> _serviceBusSenders;

        public PcsBusSender() => _serviceBusSenders = new List<KeyValuePair<string, ServiceBusSender>>();

        public void Add(string topicName, ServiceBusSender sender) => _serviceBusSenders.Add(new KeyValuePair<string, ServiceBusSender>(topicName, sender));

        public Task SendAsync(string topic, string jsonMessage)
        {
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage));
            var serviceBusSender = _serviceBusSenders.SingleOrDefault(t => t.Key == topic).Value;
            if (serviceBusSender == null)
            {
                throw new Exception($"Unable to find ServiceBusSender for topic: {topic}");
            }
            return serviceBusSender.SendMessageAsync(message);
        }

        public async Task CloseAllAsync()
        {
            foreach (var serviceBusSender in _serviceBusSenders)
            {
                await serviceBusSender.Value.CloseAsync();
            }
        }
    }
}
