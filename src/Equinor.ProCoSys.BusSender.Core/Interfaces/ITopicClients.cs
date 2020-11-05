using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSender.Core.Interfaces
{
    public interface ITopicClients
    {
        Task Send(string topic, string message);
        Task CloseAllAsync();
    }
}
