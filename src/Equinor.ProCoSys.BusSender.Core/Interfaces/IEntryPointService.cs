using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IEntryPointService
{
    Task<bool> DoWorkerJob();
    Task StopService();
}
