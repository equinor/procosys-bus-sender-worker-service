using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Topics;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class BusEventService : IBusEventService
{
    private readonly IBusSenderMessageRepository _busSenderMessageRepository;
    private readonly ITagDetailsRepository _tagDetailsRepository;

    public BusEventService(ITagDetailsRepository tagDetailsRepository,
        IBusSenderMessageRepository busSenderMessageRepository)
    {
        _tagDetailsRepository = tagDetailsRepository;
        _busSenderMessageRepository = busSenderMessageRepository;
    }

    public async Task<string> AttachTagDetails(string tagMessage)
    {
        var tagTopic = JsonSerializer.Deserialize<TagTopic>(WashString(tagMessage));
        if (tagTopic?.TagId == null || !long.TryParse(tagTopic.TagId, out var tagId))
        {
            throw new Exception("Could not deserialize TagTopic");
        }
        tagTopic.TagDetails = WashString(await _tagDetailsRepository.GetDetailsStringByTagId(tagId));
        return JsonSerializer.Serialize(tagTopic);
    }

    public async Task<string> CreateCallOffMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var callOffId)
            ? WashString(await _busSenderMessageRepository.GetCallOffMessage(callOffId))
            : throw new Exception("Failed to extract callOffId from message");

    public async Task<string> CreateChecklistMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var checkListId)
            ? WashString(await _busSenderMessageRepository.GetCheckListMessage(checkListId))
            : throw new Exception("Failed to extract checkListId from message");

    public async Task<string> CreateCommPkgQueryMessage(string message) =>
        CanGetTwoIdsFromMessage(message.Split(","), out var commPkgId, out var documentId)
            ? WashString(await _busSenderMessageRepository.GetCommPkgQueryMessage(commPkgId,documentId))
            : throw new Exception("Failed to extract checkListId from message");

    public async Task<string> CreateCommPkgOperationMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var commPkgId)
            ? WashString(await _busSenderMessageRepository.GetCommPkgOperationMessage(commPkgId))
            : throw new Exception("Failed to extract commPkgId from message");

    public async Task<string> CreateDocumentMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var documentId)
            ? WashString(await _busSenderMessageRepository.GetDocumentMessage(documentId))
            : throw new Exception("Failed to extract documentId from message");

    public async Task<string> CreateTaskMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var taskId)
            ? WashString(await _busSenderMessageRepository.GetTaskMessage(taskId))
            : throw new Exception("Failed to extract taskId from message");

    public async Task<string> CreateSwcrOtherReferencesMessage(string busEventMessage) =>
        Guid.TryParse(busEventMessage, out _)
            ? WashString(await _busSenderMessageRepository.GetSwcrOtherReferencesMessage(busEventMessage))
            : throw new Exception($"Failed to extract or parse guid SwcrOtherReferences from message {busEventMessage}");

    public async Task<string> CreateSwcrTypeMessage(string busEventMessage) =>
        Guid.TryParse(busEventMessage, out _)
            ? WashString(await _busSenderMessageRepository.GetSwcrTypeMessage(busEventMessage))
            : throw new Exception($"Failed to extract or parse guid SwcrType from message {busEventMessage}");

    public async Task<string> CreateSwcrAttachmentMessage(string busEventMessage) =>
    Guid.TryParse(busEventMessage, out _)
        ? WashString(await _busSenderMessageRepository.GetSwcrAttachmentMessage(busEventMessage))
        : throw new Exception($"Failed to extract or parse guid SwcrAttachment from message {busEventMessage}");

    public async Task<string> CreateActionMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var actionId)
            ? WashString(await _busSenderMessageRepository.GetActionMessage(actionId))
            : throw new Exception("Failed to extract actionId from message");

    public async Task<string> CreateCommPkgTaskMessage(string busEventMessage) =>
        CanGetTwoIdsFromMessage(busEventMessage.Split(","), out var commPkgId, out var taskId)
            ? WashString(await _busSenderMessageRepository.GetCommPkgTaskMessage(commPkgId,taskId))
            : throw new Exception("Failed to extract commPkgId/taskId from message");

    public async Task<string> CreateMilestoneMessage(string message) =>
        CanGetTwoIdsFromMessage(message.Split(","), out var elementId, out var milestoneId)
            ? WashString(await _busSenderMessageRepository.GetMilestoneMessage(elementId, milestoneId))
            : throw new Exception("Failed to extract element or milestone Id from message");
    public async Task<string> CreateLoopContentMessage(string busEventMessage)
        => long.TryParse(busEventMessage, out var loopContentId)
            ? WashString(await _busSenderMessageRepository.GetLoopContentMessage(loopContentId))
            : throw new Exception("Failed to extract LoopContent from message");

    public async Task<string> CreateLibraryFieldMessage(string busEventMessage) =>
        Guid.TryParse(busEventMessage, out _)
            ? WashString(await _busSenderMessageRepository.GetLibraryFieldMessage(busEventMessage))
            : throw new Exception($"Failed to extract or parse guid for LibraryField from message {busEventMessage}");

    public async Task<string> CreatePipingRevisionMessage(string busEventMessage)
        => long.TryParse(busEventMessage, out var pipingRevisionId)
            ? WashString(await _busSenderMessageRepository.GetPipingRevisionMessage(pipingRevisionId))
            : throw new Exception("Failed to extract PipeRevision from message");

    public async Task<string> CreateHeatTraceMessage(string busEventMessage)
        => long.TryParse(busEventMessage, out var heatTraceId)
            ? WashString(await _busSenderMessageRepository.GetHeatTraceMessage(heatTraceId))
            : throw new Exception("Failed to extract (heat trace)id from message");

    public async Task<string> CreatePipingSpoolMessage(string busEventMessage) =>
            long.TryParse(busEventMessage, out var pipingSpoolId)
            ? WashString(await _busSenderMessageRepository.GetPipingSpoolMessage(pipingSpoolId))
            : throw new Exception("Failed to extract PipingSpool from message");

    public async Task<string> CreateQueryMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var documentId)
            ? WashString(await _busSenderMessageRepository.GetQueryMessage(documentId))
            : throw new Exception("Failed to extract documentId from message");

    public async Task<string> CreateQuerySignatureMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var querySignatureId)
            ? WashString(await _busSenderMessageRepository.GetQuerySignatureMessage(querySignatureId))
            : throw new Exception("Failed to extract QuerySignature from message");


    public async Task<string> CreateStockMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var stockId)
            ? WashString(await _busSenderMessageRepository.GetStockMessage(stockId))
            : throw new Exception("Failed to extract stockId from message");

    public async Task<string> CreateSwcrMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var swcrId)
            ? WashString(await _busSenderMessageRepository.GetSwcrMessage(swcrId))
            : throw new Exception("Failed to extract swcrId from message");

    public async Task<string> CreateSwcrSignatureMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var swcrSignatureId)
            ? WashString(await _busSenderMessageRepository.GetSwcrSignatureMessage(swcrSignatureId))
            : throw new Exception("Failed to extract swcrSignatureId from message");

    public async Task<string> CreateWoChecklistMessage(string busEventMessage) =>
        CanGetTwoIdsFromMessage(busEventMessage.Split(","), out var tagCheckId, out var woId)
            ? WashString(await _busSenderMessageRepository.GetWorkOrderChecklistMessage(tagCheckId, woId))
            : throw new Exception("Failed to extract Wo or Checklist Id from message");

    public async Task<string> CreateWoMaterialMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var workOrderId)
            ? WashString(await _busSenderMessageRepository.GetWorkOrderMaterialMessage(workOrderId))
            : throw new Exception("Failed to extract workOrderId from message");

    public async Task<string> CreateWoMilestoneMessage(string message) =>
        CanGetTwoIdsFromMessage(message.Split(","), out var workOrderId, out var milestoneId)
            ? WashString(await _busSenderMessageRepository.GetWorkOrderMilestoneMessage(workOrderId, milestoneId))
            : throw new Exception("Failed to extract WorkOrder or Milestone Id from message");

    public async Task<string> CreateWorkOrderMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var workOrderId)
            ? WashString(await _busSenderMessageRepository.GetWorkOrderMessage(workOrderId))
            : throw new Exception("Failed to extract workOrderId from message");

    public async Task<string> CreateWorkOrderCutOffMessage(string message)
    {
        var  woInfo = message.Split(","); // woId,cutoffweek
        if (woInfo.Length != 2)
        {
            throw new Exception("Failed to extract workOrderId and cutoffweek from message");
        }
        return long.TryParse(woInfo[0], out var workOrderId)
            ? WashString(await _busSenderMessageRepository.GetWorkOrderCutOffMessage(workOrderId,woInfo[1]))
            : throw new Exception("Failed to extract workOrderId from message");
    }

    public string WashString(string busEventMessage)
    {
        if (string.IsNullOrEmpty(busEventMessage))
        {
            return busEventMessage;
        }

        /***
         *  This code is duplicated in FamWebJob (TieMapper)
         *  https://stackoverflow.com/questions/6198986/how-can-i-replace-non-printable-unicode-characters-in-java
         *  https://regex101.com/
         *
         * '\p{C}'  matches invisible control characters and unused code points
         *  '+'     matches the previous token between one and unlimited times, as many times as possible, giving back as needed (greedy)
         */
        busEventMessage = Regex.Replace(busEventMessage, @"\p{C}+", string.Empty);

        return busEventMessage;
    }

    public static bool CanGetTwoIdsFromMessage(IReadOnlyList<string> array, out long id1, out long id2)
    {
        id1 = 0;
        id2 = 0;
        return array.Count == 2
               && long.TryParse(array[0], out id1)
               && long.TryParse(array[1], out id2);
    }
}
