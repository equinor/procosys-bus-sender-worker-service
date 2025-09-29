using System.Threading.Tasks;
using Dapper;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IEventRepository
{
    Task<T?> QuerySingle<T>((string queryString, DynamicParameters parameters) query, string objectId) where T : IHasEventType;
    Task<string> QuerySimple((string queryString, DynamicParameters parameters) query, long objectId);
}
