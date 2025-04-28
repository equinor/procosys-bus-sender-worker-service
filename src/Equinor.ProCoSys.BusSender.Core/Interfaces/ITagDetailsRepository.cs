using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface ITagDetailsRepository
{
    public Task<Dictionary<long, string>> GetDetailsByTagId(IEnumerable<long> tagIds);
}
