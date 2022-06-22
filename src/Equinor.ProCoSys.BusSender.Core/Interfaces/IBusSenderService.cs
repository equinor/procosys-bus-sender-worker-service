using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IBusSenderService
{
    Task HandleBusEvents();
    Task CloseConnections();
}
