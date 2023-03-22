﻿using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IBusSenderMessageRepository
{
    Task<string?> GetCallOffMessage(long callOffId);
    Task<string?> GetCommPkgQueryMessage(long commPkgId, long documentId);
    Task<string?> GetDocumentMessage(long documentId);
    Task<string?> GetTaskMessage(long taskId);
    Task<string?> GetSwcrOtherReferenceMessage(string guid);
    Task<string?> GetSwcrTypeMessage(string guid);
    Task<string?> GetSwcrAttachmentMessage(string guid);
    Task<string?> GetActionMessage(long actionId);
    Task<string?> GetCommPkgTaskMessage(long commPkgId, long taskId);
    Task<string?> GetLoopContentMessage(long loopContentId);
    Task<string?> GetPipingRevisionMessage(long pipeRevisionId);
    Task<string?> GetPipingSpoolMessage(long pipingSpoolId);
    Task<string?> GetQueryMessage(long documentId);
    Task<string?> GetQuerySignatureMessage(long querySignatureId);
    Task<string?> GetStockMessage(long stockId);
    Task<string?> GetSwcrMessage(long swcrId);
    Task<string?> GetSwcrSignatureMessage(long swcrSignatureId);
    Task<string?> GetWorkOrderChecklistMessage(long tagCheckId, long workOrderId);
    Task<string?> GetWorkOrderMaterialMessage(long workOrderId);
    //Task<string> GetWorkOrderMilestoneMessage(long woId, long milestoneId);
    Task<string?> GetWorkOrderCutOffMessage(long workOrderId, string cutoffWeek);
    Task<string?> GetHeatTraceMessage(long id);
    Task<string?> GetCommPkgOperationMessage(long commPkgId);
    Task<string?> GetLibraryFieldMessage(string guid);
}
