using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSender.Core.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}
