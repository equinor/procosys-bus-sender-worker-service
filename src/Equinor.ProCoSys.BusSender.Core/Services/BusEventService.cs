using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Extensions;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.PcsServiceBus.Queries;
using Equinor.ProCoSys.PcsServiceBus.Topics;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class BusEventService : IBusEventService
{
    private readonly IBusSenderMessageRepository _busSenderMessageRepository;
    private readonly IDapperRepository _dapperRepository;
    private readonly ITagDetailsRepository _tagDetailsRepository;

    public BusEventService(ITagDetailsRepository tagDetailsRepository,
        IBusSenderMessageRepository busSenderMessageRepository, IDapperRepository dapperRepository)
    {
        _tagDetailsRepository = tagDetailsRepository;
        _busSenderMessageRepository = busSenderMessageRepository;
        _dapperRepository = dapperRepository;
    }

    public async Task<string> AttachTagDetails(string? tagMessage)
    {
        var tagTopic =
            JsonSerializer.Deserialize<TagTopic>(WashString(tagMessage) ?? throw new InvalidOperationException());
        if (tagTopic?.TagId == null || !long.TryParse(tagTopic.TagId, out var tagId))
        {
            throw new Exception("Could not deserialize TagTopic");
        }

        tagTopic.TagDetails = WashString(await _tagDetailsRepository.GetDetailsStringByTagId(tagId));
        return JsonSerializer.Serialize(tagTopic);
    }

    public async Task<string?> CreateActionMessage(string message)
    {
        if (!long.TryParse(message, out var actionId))
        {
            throw new Exception($"Failed to extract guid from message {message}");
        }

        var queryString = ActionQuery.GetQuery(actionId);
        var actionEvents = await _dapperRepository.QuerySingle<ActionEvent>(queryString, message);
        return JsonSerializer.Serialize(actionEvents, DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateCallOffMessage(string message) =>
        long.TryParse(message, out var callOffId)
            ? JsonSerializer.Serialize(
                await _dapperRepository.QuerySingle<CallOffEvent>(CallOffQuery.GetQuery(callOffId),
                    callOffId.ToString()), DefaultSerializerHelper.SerializerOptions)
            : throw new Exception("Failed to extract checklistId from message");

    public async Task<string?> CreateChecklistMessage(string message) =>
        long.TryParse(message, out var checklistId)
            ? JsonSerializer.Serialize(
                await _dapperRepository.QuerySingle<ChecklistEvent>(ChecklistQuery.GetQuery(checklistId),
                    checklistId.ToString()))
            : throw new Exception($"Failed to extract checklistId from message: {message}");
    
    public async Task<string> CreateCommPkgMessage(string message)
        => long.TryParse(message, out var commPkgId)
            ? JsonSerializer.Serialize(
                await _dapperRepository.QuerySingle<CommPkgEvent>(CommPkgQuery.GetQuery(commPkgId),
                    commPkgId.ToString()))
            : throw new Exception($"Failed to extract checklistId from message: {message}");

    public async Task<string?> CreateCommPkgMilestoneMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract guid from message: {message}");
        }

        var queryString = CommPkgMilestoneQuery.GetQuery(message);
        var mcPkgMilestoneEvents = await _dapperRepository.QuerySingle<CommPkgMilestoneEvent>(queryString, message);
        return JsonSerializer.Serialize(mcPkgMilestoneEvents, DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateCommPkgOperationMessage(string message) =>
        long.TryParse(message, out var commPkId)
            ? JsonSerializer.Serialize(
                await _dapperRepository.QuerySingle<CommPkgOperationEvent>(CommPkgOperationQuery.GetQuery(commPkId),
                    commPkId.ToString()))
            : throw new Exception($"Failed to extract checklistId from message: {message}");

    public async Task<string?> CreateCommPkgQueryMessage(string message) =>
        CanGetTwoIdsFromMessage(message.Split(","), out var commPkgId, out var documentId)
            ? JsonSerializer.Serialize(
                await _dapperRepository.QuerySingle<CommPkgQueryEvent>(
                    QueryCommPkgQuery.GetQuery(commPkgId, documentId), message))
            : throw new Exception($"Failed to extract commPkgId or TaskId from message: {message}");

    public async Task<string?> CreateCommPkgTaskMessage(string message) =>
        CanGetTwoIdsFromMessage(message.Split(","), out var commPkgId, out var taskId)
            ? JsonSerializer.Serialize(
                await _dapperRepository.QuerySingle<CommPkgTaskEvent>(
                    CommPkgTaskQuery.GetQuery(commPkgId, taskId), message))
            : throw new Exception($"Failed to extract commPkgId and/or taskId from message: {message}");

    public async Task<string?> CreateDocumentMessage(string message) =>
        long.TryParse(message, out var documentId)
            ? WashString(await _busSenderMessageRepository.GetDocumentMessage(documentId))
            : throw new Exception($"Failed to extract documentId from message: {message}");

    public async Task<string?> CreateHeatTraceMessage(string message)
        => long.TryParse(message, out var heatTraceId)
            ? JsonSerializer.Serialize(
                await _dapperRepository.QuerySingle<HeatTraceEvent>(
                    HeatTraceQuery.GetQuery(heatTraceId), message))
            : throw new Exception($"Failed to extract heatTraceId from message: {message}");
    
    public async Task<string> CreateLibraryMessage(string message)
        => long.TryParse(message, out var libraryId)
            ? JsonSerializer.Serialize(
                await _dapperRepository.QuerySingle<LibraryEvent>(LibraryQuery.GetQuery(libraryId),
                    libraryId.ToString()))
            : throw new Exception($"Failed to extract checklistId from message: {message}");

    public async Task<string> CreateMcPkgMessage(string message) => 
        long.TryParse(message, out var mcPkgId)
            ? JsonSerializer.Serialize(
                await _dapperRepository.QuerySingle<McPkgEvent>(McPkgQuery.GetQuery(mcPkgId),
                    mcPkgId.ToString()))
            : throw new Exception($"Failed to extract mcPkgId from message: {message}");

    public async Task<string> CreateProjectMessage(string message) =>
        long.TryParse(message, out var projectId)
            ? JsonSerializer.Serialize(
                await _dapperRepository.QuerySingle<ProjectEvent>(ProjectQuery.GetQuery(projectId),
                    projectId.ToString()))
            : throw new Exception($"Failed to extract projectId from message: {message}");

    public async Task<string?> CreateLibraryFieldMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract libraryField(elementcontent) guid from message {message}");
        }
        var queryString = LibraryFieldQuery.GetQuery(message);
        var libraryFieldEvents = await _dapperRepository.QuerySingle<LibraryFieldEvent>(queryString, message);
        return JsonSerializer.Serialize(libraryFieldEvents, DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateLoopContentMessage(string message)
        => long.TryParse(message, out var loopContentId)
            ? WashString(await _busSenderMessageRepository.GetLoopContentMessage(loopContentId))
            : throw new Exception($"Failed to extract LoopContent from message: {message}");

    public async Task<string?> CreateMcPkgMilestoneMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract guid from message {message}");
        }

        var queryString = McPkgMilestoneQuery.GetQuery(message);
        var mcPkgMilestoneEvents = await _dapperRepository.QuerySingle<McPkgMilestoneEvent>(queryString, message);
        return JsonSerializer.Serialize(mcPkgMilestoneEvents, DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreatePipingRevisionMessage(string message)
        => long.TryParse(message, out var pipingRevisionId)
            ? WashString(await _busSenderMessageRepository.GetPipingRevisionMessage(pipingRevisionId))
            : throw new Exception($"Failed to extract PipeRevision from message: {message}");

    public async Task<string?> CreatePipingSpoolMessage(string message) =>
        long.TryParse(message, out var pipingSpoolId)
            ? WashString(await _busSenderMessageRepository.GetPipingSpoolMessage(pipingSpoolId))
            : throw new Exception($"Failed to extract PipingSpool from message: {message}");

    public async Task<string?> CreateQueryMessage(string message) =>
        long.TryParse(message, out var documentId)
            ? WashString(await _busSenderMessageRepository.GetQueryMessage(documentId))
            : throw new Exception($"Failed to extract documentId from message: {message}");

    public async Task<string?> CreateQuerySignatureMessage(string message) =>
        long.TryParse(message, out var querySignatureId)
            ? WashString(await _busSenderMessageRepository.GetQuerySignatureMessage(querySignatureId))
        : throw new Exception($"Failed to extract QuerySignature from message: {message}");


    public async Task<string?> CreateStockMessage(string message) =>
        long.TryParse(message, out var stockId)
            ? WashString(await _busSenderMessageRepository.GetStockMessage(stockId))
            : throw new Exception($"Failed to extract stockId from message: {message}");

    public async Task<string?> CreateSwcrAttachmentMessage(string message) =>
        Guid.TryParse(message, out _)
            ? WashString(await _busSenderMessageRepository.GetSwcrAttachmentMessage(message))
            : throw new Exception($"Failed to extract or parse guid SwcrAttachment from message {message}");

    public async Task<string?> CreateSwcrMessage(string message) =>
        long.TryParse(message, out var swcrId)
            ? WashString(await _busSenderMessageRepository.GetSwcrMessage(swcrId))
            : throw new Exception($"Failed to extract swcrId from message: {message}");

    public async Task<string?> CreateSwcrOtherReferenceMessage(string message) =>
        Guid.TryParse(message, out _)
            ? WashString(await _busSenderMessageRepository.GetSwcrOtherReferenceMessage(message))
            : throw new Exception(
                $"Failed to extract or parse guid SwcrOtherReferences from message {message}");

    public async Task<string?> CreateSwcrSignatureMessage(string message) =>
        long.TryParse(message, out var swcrSignatureId)
            ? WashString(await _busSenderMessageRepository.GetSwcrSignatureMessage(swcrSignatureId))
            : throw new Exception($"Failed to extract swcrSignatureId from message: {message}");

    public async Task<string?> CreateSwcrTypeMessage(string message) =>
        Guid.TryParse(message, out _)
            ? WashString(await _busSenderMessageRepository.GetSwcrTypeMessage(message))
            : throw new Exception($"Failed to extract or parse guid SwcrType from message {message}");

    public async Task<string?> CreateTaskMessage(string message) =>
        long.TryParse(message, out var taskId)
            ? JsonSerializer.Serialize(
                await _dapperRepository.QuerySingle<TaskEvent>(TaskQuery.GetQuery(taskId), taskId.ToString()))
            : throw new Exception($"Failed to extract checklistId from message: {message}");

    public async Task<string?> CreateWoChecklistMessage(string message) =>
        CanGetTwoIdsFromMessage(message.Split(","), out var tagCheckId, out var woId)
            ? WashString(await _busSenderMessageRepository.GetWorkOrderChecklistMessage(tagCheckId, woId))
            : throw new Exception($"Failed to extract Wo or Checklist Id from message: {message}");

    public async Task<string?> CreateWoMaterialMessage(string message) =>
        long.TryParse(message, out var workOrderId)
            ? WashString(await _busSenderMessageRepository.GetWorkOrderMaterialMessage(workOrderId))
            : throw new Exception($"Failed to extract workOrderId from message: {message}");

    public async Task<string?> CreateWoMilestoneMessage(string message)
    {
        if (CanGetTwoIdsFromMessage(message.Split(","), out var workOrderId, out var milestoneId))
        {
            throw new Exception($"Failed to extract guid from message {message}");
        }

        var queryString = WorkOrderMilestoneQuery.GetQuery(workOrderId, milestoneId);
        var workOrderMilestoneEvent =
            await _dapperRepository.QuerySingle<WorkOrderMilestoneEvent>(queryString, message);
        return JsonSerializer.Serialize(workOrderMilestoneEvent, DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateWorkOrderCutOffMessage(string message)
    {
        var woInfo = message.Split(","); // woId,cutoffweek
        if (woInfo.Length != 2)
        {
            throw new Exception($"Failed to extract workOrderId and cutoffweek from message: {message}");
        }

        return long.TryParse(woInfo[0], out var workOrderId)
            ? WashString(await _busSenderMessageRepository.GetWorkOrderCutOffMessage(workOrderId, woInfo[1]))
            : throw new Exception($"Failed to extract workOrderId from message: {message}");
    }

    public async Task<string?> CreateWorkOrderMessage(string message)
    {
        if (!long.TryParse(message, out var workOrderId))
        {
            throw new Exception($"Failed to extract guid from message {message}");
        }

        var queryString = WorkOrderQuery.GetQuery(workOrderId);
        var workOrderEvent = await _dapperRepository.QuerySingle<WorkOrderEvent>(queryString, message);
        return JsonSerializer.Serialize(workOrderEvent, DefaultSerializerHelper.SerializerOptions);
    }

    public string? WashString(string? message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return message;
        }

        /***
         *  This code is duplicated in FamWebJob (TieMapper)
         *  https://stackoverflow.com/questions/6198986/how-can-i-replace-non-printable-unicode-characters-in-java
         *  https://regex101.com/
         *
         * '\p{C}'  matches invisible control characters and unused code points
         *  '+'     matches the previous token between one and unlimited times, as many times as possible, giving back as needed (greedy)
         */
        message = Regex.Replace(message, @"\p{C}+", string.Empty);

        return message;
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
