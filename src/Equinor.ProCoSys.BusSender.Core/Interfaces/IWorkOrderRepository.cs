using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IWorkOrderRepository
{
    Task<string> GetWorkOrderMessage(long workOrderId);
}
