

using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces
{
    public interface ITagDetailsRepository
    {
        public Task<string> GetByTagId(long tagId);
    }
}
