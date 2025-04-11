using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IBusEventService
{
    Task<string> AttachTagDetails(string? tagMessage);
    Task AttachTagDetails(List<BusEvent> busEvent);
    Task<string?> CreateActionMessage(string busEventMessage);
    Task<string?> CreateCallOffMessage(string busEventMessage);
    Task<string?> CreateChecklistMessage(string busEventMessage);
    Task<string?> CreateCommPkgMessage(string message);
    Task<string?> CreateCommPkgMilestoneMessage(string busEventMessage);
    Task<string?> CreateCommPkgOperationMessage(string busEventMessage);
    Task<string?> CreateCommPkgQueryMessage(string busEventMessage);
    Task<string?> CreateCommPkgTaskMessage(string busEventMessage);
    Task<string?> CreateDocumentMessage(string busEventMessage);
    Task<string?> CreateHeatTraceMessage(string busEventMessage);
    Task<string?> CreatePersonMessage(string busEventMessage);

    Task<string?> CreateLibraryFieldMessage(string busEventMessage);
    Task<string?> CreateLibraryMessage(string message);
    Task<string?> CreateLoopContentMessage(string busEventMessage);
    Task<string?> CreateMcPkgMessage(string message);
    Task<string?> CreateMcPkgMilestoneMessage(string message);
    Task<string?> CreatePipingRevisionMessage(string busEventMessage);
    Task<string?> CreatePipingSpoolMessage(string busEventMessage);
    Task<string?> CreateProjectMessage(string message);
    Task<string?> CreatePunchListItemMessage(string message);
    Task<string?> CreateQueryMessage(string busEventMessage);
    Task<string?> CreateQuerySignatureMessage(string busEventMessage);
    Task<string?> CreateResponsibleMessage(string message);
    Task<string?> CreateStockMessage(string busEventMessage);
    Task<string?> CreateSwcrAttachmentMessage(string busEventMessage);
    Task<string?> CreateSwcrMessage(string busEventMessage);
    Task<string?> CreateSwcrOtherReferenceMessage(string busEventMessage);
    Task<string?> CreateSwcrSignatureMessage(string busEventMessage);
    Task<string?> CreateSwcrTypeMessage(string busEventMessage);
    Task<string?> CreateTagEquipmentMessage(string busEventMessage);
    Task<string?> CreateTaskMessage(string busEventMessage);
    Task<string?> CreateWorkOrderChecklistMessage(string busEventMessage);
    Task<string?> CreateWorkOrderCutoffMessage(string message);
    Task<string?> CreateWorkOrderMaterialMessage(string busEventMessage);
    Task<string?> CreateWorkOrderMessage(string busEventMessage);
    Task<string?> CreateWorkOrderMilestoneMessage(string message);
    Task<string?> CreateHeatTracePipeTestMessage(string message);
    Task<string?> CreateNotificationMessage(string message);
    Task<string?> CreateNotificationWorkOrderMessage(string message);
    Task<string?> CreateNotificationCommPkgMessage(string message);
    Task<string?> CreateNotificationSignatureMessage(string message);

    string? WashString(string? message);
    Task<string?> CreatePunchPriorityLibRelationMessage(string message);
}
