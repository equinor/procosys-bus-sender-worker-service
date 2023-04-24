﻿using System.IO;
using System.Threading.Tasks;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IBusEventService
{
    Task<string> AttachTagDetails(string? tagMessage);
    string? WashString(string? message);
    Task<string?> CreateQueryMessage(string busEventMessage);
    Task<string?> CreateQuerySignatureMessage(string busEventMessage);
    Task<string?> CreateDocumentMessage(string busEventMessage);
    Task<string?> CreateTaskMessage(string busEventMessage);
    Task<string?> CreateSwcrOtherReferenceMessage(string busEventMessage);
    Task<string?> CreateSwcrTypeMessage(string busEventMessage);
    Task<string?> CreateSwcrAttachmentMessage(string busEventMessage);
    Task<string?> CreateActionMessage(string busEventMessage);
    Task<string?> CreateCommPkgTaskMessage(string busEventMessage);
    Task<string?> CreateWorkOrderMessage(string busEventMessage);
    Task<string?> CreateChecklistMessage(string busEventMessage);
    Task<string?> CreateCallOffMessage(string busEventMessage);
    Task<string?> CreateWoChecklistMessage(string busEventMessage);
    Task<string?> CreateWoMilestoneMessage(string message);
    Task<string?> CreateWoMaterialMessage(string busEventMessage);
    Task<string?> CreateStockMessage(string busEventMessage);
    Task<string?> CreateSwcrMessage(string busEventMessage);
    Task<string?> CreateSwcrSignatureMessage(string busEventMessage);
    Task<string?> CreatePipingRevisionMessage(string busEventMessage);
    Task<string?> CreatePipingSpoolMessage(string busEventMessage);
    Task<string?> CreateLoopContentMessage(string busEventMessage);
    Task<string?> CreateCommPkgQueryMessage(string busEventMessage);
    Task<string?> CreateWorkOrderCutoffMessage(string message);
    Task<string?> CreateMcPkgMilestoneMessage(string message);
    Task<string?> CreateHeatTraceMessage(string busEventMessage);
    Task<string?> CreateCommPkgOperationMessage(string busEventMessage);

    Task<string?> CreateLibraryFieldMessage(string busEventMessage);
    Task<string?> CreateCommPkgMilestoneMessage(string busEventMessage);
    Task<string> CreateCommPkgMessage(string message);
    Task<string> CreateLibraryMessage(string message);
    Task<string> CreateMcPkgMessage(string message);
    Task<string> CreateProjectMessage(string message);
    Task<string> CreatePunchListItemMessage(string message);
}
