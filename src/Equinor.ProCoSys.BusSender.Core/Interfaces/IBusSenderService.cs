using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces
{
    public interface IBusSenderService
    {
        Task SendMessageChunk();
        Task StopService();
    }
}
