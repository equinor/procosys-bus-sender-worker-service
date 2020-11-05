using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSender.Core.Interfaces
{
    public interface IBusSenderService
    {
        Task SendMessageChunk();
        Task StopService();
    }
}
