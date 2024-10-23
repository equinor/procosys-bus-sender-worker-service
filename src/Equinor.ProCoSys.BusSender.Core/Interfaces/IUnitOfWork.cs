using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();

    void ClearChangeTracker();
}
