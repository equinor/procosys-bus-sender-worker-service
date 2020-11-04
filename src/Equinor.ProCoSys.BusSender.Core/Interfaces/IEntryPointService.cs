using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSender.Core.Interfaces
{
    public interface IEntryPointService
    {
        Task SendMessageChunk();
        Task StopService();
    }
}
