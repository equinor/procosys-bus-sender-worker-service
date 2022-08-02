using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IBusSenderMessageRepository
{
    Task<string> GetCallOffMessage(long callOffId);
    Task<string> GetCheckListMessage(long checkListId);
    Task<string> GetCommPkgQueryMessage(long commPkgId, long documentId);
    Task<string> GetDocumentMessage(long documentId);
    Task<string> GetLoopContentMessage(long loopContentId);
    Task<string> GetPipingRevisionMessage(long pipeRevisionId);
    Task<string> GetPipingSpoolMessage(long pipingSpoolId);
    Task<string> GetQueryMessage(long documentId);
    Task<string> GetQuerySignatureMessage(long querySignatureId);
    Task<string> GetStockMessage(long stockId);
    Task<string> GetSwcrMessage(long swcrId);
    Task<string> GetSwcrSignatureMessage(long swcrSignatureId);
    Task<string> GetWorkOrderChecklistMessage(long tagCheckId, long workOrderId);
    Task<string> GetWorkOrderMaterialMessage(long workOrderId);
    Task<string> GetWorkOrderMessage(long workOrderId);
    Task<string> GetWorkOrderMilestoneMessage(long woId, long milestoneId);
}
