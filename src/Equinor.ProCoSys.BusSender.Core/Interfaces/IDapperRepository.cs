
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IDapperRepository
{
    Task<IEnumerable<T>> Query<T>(string queryString, string? objectId) where T : IHasEventType;
}
