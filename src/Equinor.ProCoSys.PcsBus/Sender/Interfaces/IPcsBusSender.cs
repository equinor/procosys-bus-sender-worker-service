using System.Threading.Tasks;

namespace Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces
{
    public interface IPcsBusSender
    {
        Task SendAsync(string topic, string jsonMessage);
        Task CloseAllAsync();
    }
}
