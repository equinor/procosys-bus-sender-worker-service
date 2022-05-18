using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IDocumentRepository
{
    Task<string> GetQueryMessage(long documentId);

    Task<string> GetDocumentMessage(long documentId);
}
