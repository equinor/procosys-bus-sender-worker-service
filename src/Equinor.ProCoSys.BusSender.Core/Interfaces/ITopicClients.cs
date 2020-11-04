using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Equinor.ProCoSys.BusSender.Core.Interfaces
{
    public interface ITopicClients
    {
        void Add(string topicName, ITopicClient topicClient);
        Task Send(string topic, string message);
        Task CloseAllAsync();
    }
}
