using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface ITagDetailsRepository
{
    public Task<string> GetDetailsStringByTagId(long tagId);
}
