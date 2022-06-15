using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IDocumentRepository : IRepository
{
    Task<string> GetQueryMessage(long documentId);

    Task<string> GetDocumentMessage(long documentId);
}
