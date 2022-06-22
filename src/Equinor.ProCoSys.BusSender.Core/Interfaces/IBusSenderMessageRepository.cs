using System.Collections.Generic;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IBusSenderMessageRepository
{
    Task<string> GetWorkOrderMessage(long workOrderId);
    Task<string> GetQueryMessage(long documentId);
    Task<string> GetDocumentMessage(long documentId);
    Task<string> GetCheckListMessage(long checkListId);
}
