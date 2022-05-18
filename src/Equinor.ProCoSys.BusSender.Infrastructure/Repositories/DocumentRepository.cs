using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class DocumentRepository : IDocumentRepository
{
    public Task<string> GetQueryMessage(long documentId)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetDocumentMessage(long documentId)
    {
        throw new NotImplementedException();
    }
}
