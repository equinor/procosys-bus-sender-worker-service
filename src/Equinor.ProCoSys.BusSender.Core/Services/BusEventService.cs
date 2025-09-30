﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    private readonly IEventRepository _eventRepository;
    private readonly ITagDetailsRepository _tagDetailsRepository;

    public BusEventService(ITagDetailsRepository tagDetailsRepository,
        IEventRepository eventRepository)
    {
        _tagDetailsRepository = tagDetailsRepository;
        _eventRepository = eventRepository;
    }

    public async Task AttachTagDetails(BusEvent busEvent) => await AttachTagDetails([busEvent]);

    public async Task AttachTagDetails(List<BusEvent> busEvents)
    {
        var tagTopics = busEvents
            .Select(x => (Event: x, Topic: DeserializeTagTopic(x.Message)))
            .ToList();

        // There could be multiple updates for the same tagId within a short time span.
        // For simple messages duplicates are filtered out, but this is not the case for thick messages like this.
        // We only need to fetch the tag details once for each tagId when handled within the same loop.
        // That is why we use distinct here.
        var tagIds = tagTopics
            .Select(x => long.Parse(x.Topic.TagId))
            .Distinct()
            .ToList();
        
        var tagDetailsDictionary = await _tagDetailsRepository.GetDetailsByTagId(tagIds);

        foreach (var (busEvent, tagTopic) in tagTopics)
        {
            var tagId = long.Parse(tagTopic.TagId);
            tagTopic.TagDetails = tagDetailsDictionary.TryGetValue(tagId, out var details)
                ? WashString(details)
                : throw new KeyNotFoundException($"Tag details not available in dictionary for TagId: {tagId}. This should never happen.");

            busEvent.Message = JsonSerializer.Serialize(tagTopic);
        }
    }

    private TagTopic DeserializeTagTopic(string? message)
    {
        var washedMessage = WashString(message) ?? throw new InvalidOperationException("Message is null or empty");
        var tagTopic = JsonSerializer.Deserialize<TagTopic>(washedMessage);

        if (tagTopic?.TagId == null || !long.TryParse(tagTopic.TagId, out var _))
        {
            throw new Exception($"Could not deserialize TagTopic. TagId: {tagTopic?.TagId}");
        }
        return tagTopic;
    }

    public static bool CanGetTwoIdsFromMessage(IReadOnlyList<string> array, out long id1, out long id2)
    {
        id1 = 0;
        id2 = 0;
        return array.Count == 2
               && long.TryParse(array[0], out id1)
               && long.TryParse(array[1], out id2);
    }

    public async Task<string?> CreateActionMessage(string message)
    {
        if (!long.TryParse(message, out var actionId))
        {
            throw new Exception($"Failed to extract guid from message {message}");
        }

        var queryString = ActionQuery.GetQuery(actionId);
        var actionEvents = await _eventRepository.QuerySingle<ActionEvent>(queryString, message);
        return JsonSerializer.Serialize(actionEvents, DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateCallOffMessage(string message)
    {
        if (!long.TryParse(message, out var callOffId))
        {
            throw new Exception("Failed to extract checklistId from message");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<CallOffEvent>(CallOffQuery.GetQuery(callOffId),
                callOffId.ToString()), DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateChecklistMessage(string message)
    {
        if (!long.TryParse(message, out var checklistId))
        {
            throw new Exception($"Failed to extract checklistId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<ChecklistEvent>(ChecklistQuery.GetQuery(checklistId),
                checklistId.ToString()));

    }

    public async Task<string?> CreateCommPkgMessage(string message)
    {
        if (!long.TryParse(message, out var commPkgId))
        {
            throw new Exception($"Failed to extract checklistId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<CommPkgEvent>(CommPkgQuery.GetQuery(commPkgId),
                commPkgId.ToString()));
    }

    public async Task<string?> CreateCommPkgMilestoneMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract guid from message: {message}");
        }

        var queryString = CommPkgMilestoneQuery.GetQuery(message);
        var mcPkgMilestoneEvents = await _eventRepository.QuerySingle<CommPkgMilestoneEvent>(queryString, message);
        return JsonSerializer.Serialize(mcPkgMilestoneEvents, DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateCommPkgOperationMessage(string message)
    {
        if (!long.TryParse(message, out var commPkId))
        {
            throw new Exception($"Failed to extract checklistId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<CommPkgOperationEvent>(CommPkgOperationQuery.GetQuery(commPkId),
                commPkId.ToString()));

    }

    public async Task<string?> CreateCommPkgQueryMessage(string message)
    {
        if (!CanGetTwoIdsFromMessage(message.Split(","), out var commPkgId, out var documentId))
        {
            throw new Exception($"Failed to extract commPkgId or TaskId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<CommPkgQueryEvent>(
                CommPkgQueryQuery.GetQuery(commPkgId, documentId), message));
    }

    public async Task<string?> CreateCommPkgTaskMessage(string message)
    {
        if (!CanGetTwoIdsFromMessage(message.Split(","), out var commPkgId, out var taskId))
        {
            throw new Exception($"Failed to extract commPkgId and/or taskId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<CommPkgTaskEvent>(
                CommPkgTaskQuery.GetQuery(commPkgId, taskId), message));
    }

    public async Task<string?> CreateDocumentMessage(string message)
    {
        if (!long.TryParse(message, out var documentId))
        {
            throw new Exception($"Failed to extract documentId from message: {message}");
        }

        var documentEvent = await _eventRepository.QuerySingle<DocumentEvent>(DocumentQuery.GetQuery(documentId),
            documentId.ToString());
        var installationCode = await _eventRepository.QuerySingleOrDefault<string>(DocumentQuery.GetInstallationCodeQuery(documentId),documentId);
        if (documentEvent is  null)
        {
            return JsonSerializer.Serialize(documentEvent);
        }
        var documentEventWithInstallationCode = documentEvent with {InstallationCode = installationCode}; 
        return JsonSerializer.Serialize(
            documentEventWithInstallationCode, DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateHeatTraceMessage(string message)
    {
        if (!long.TryParse(message, out var heatTraceId))
        {
            throw new Exception($"Failed to extract heatTraceId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<HeatTraceEvent>(
                HeatTraceQuery.GetQuery(heatTraceId), message));
    }

    public async Task<string?> CreatePersonMessage(string message)
    {
        if (!Guid.TryParse(message, out var _))
        {
            throw new Exception($"Failed to extract person guid from message: {message}");
        }

        var queryString = PersonQuery.GetQuery(message);
        var personEvents = await _eventRepository.QuerySingle<PersonEvent>(queryString, message);
        return JsonSerializer.Serialize(personEvents, DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateLibraryFieldMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract libraryField(elementcontent) guid from message {message}");
        }

        var queryString = LibraryFieldQuery.GetQuery(message);
        var libraryFieldEvents = await _eventRepository.QuerySingle<LibraryFieldEvent>(queryString, message);
        return JsonSerializer.Serialize(libraryFieldEvents, DefaultSerializerHelper.SerializerOptions);
    }
    
    public async Task<string?> CreateLibraryToLibraryMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract libraryToLibrary guid from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<LibraryToLibraryEvent>(
                LibraryToLibraryQuery.GetQuery(message), message), DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateLibraryMessage(string message)
    {
        if (!long.TryParse(message, out var libraryId))
        {
            throw new Exception($"Failed to extract checklistId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<LibraryEvent>(LibraryQuery.GetQuery(libraryId),
                libraryId.ToString()));

    }

    public async Task<string?> CreateLoopContentMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract LoopContent from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<LoopContentEvent>(
                LoopContentQuery.GetQuery(message), message));
    }

    public async Task<string?> CreateMcPkgMessage(string message)
    {
        if (!long.TryParse(message, out var mcPkgId))
        {
            throw new Exception($"Failed to extract mcPkgId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<McPkgEvent>(McPkgQuery.GetQuery(mcPkgId),
                mcPkgId.ToString()));
    }

    public async Task<string?> CreateMcPkgMilestoneMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract guid from message {message}");
        }

        var queryString = McPkgMilestoneQuery.GetQuery(message);
        var mcPkgMilestoneEvents = await _eventRepository.QuerySingle<McPkgMilestoneEvent>(queryString, message);
        return JsonSerializer.Serialize(mcPkgMilestoneEvents, DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreatePipingRevisionMessage(string message)
    {
        if (!long.TryParse(message, out var pipingRevisionId))
        {
            throw new Exception($"Failed to extract PipeRevision from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<PipingRevisionEvent>(
                PipingRevisionQuery.GetQuery(pipingRevisionId), message));
    }

    public async Task<string?> CreatePipingSpoolMessage(string message)
    {
        if (!long.TryParse(message, out var pipingSpoolId))
        {
            throw new Exception($"Failed to extract PipingSpool from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<PipingSpoolEvent>(
                PipingSpoolQuery.GetQuery(pipingSpoolId), message), DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateProjectMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract or parse Project Guid from message {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<ProjectEvent>(ProjectQuery.GetQuery(message),
                message));
    }


    public async Task<string?> CreatePunchListItemMessage(string message)
    {
        if (!long.TryParse(message, out var punchListItemId))
        {
            throw new Exception($"Failed to extract punchListItemId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<PunchListItemEvent>(PunchListItemQuery.GetQuery(punchListItemId),
                punchListItemId.ToString()));
    }

    public async Task<string?> CreateQueryMessage(string message)
    {
        if (!long.TryParse(message, out var documentId))
        {
            throw new Exception($"Failed to extract documentId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<QueryEvent>(QueryQuery.GetQuery(documentId),
                documentId.ToString()), DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateQuerySignatureMessage(string message)
    {
        if (!long.TryParse(message, out var querySignatureId))
        {
            throw new Exception($"Failed to extract QuerySignature from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<QuerySignatureEvent>(
                QuerySignatureQuery.GetQuery(querySignatureId), message));

    }

    public async Task<string?> CreateResponsibleMessage(string message)
    {
        if (!long.TryParse(message, out var responsibleId))
        {
            throw new Exception($"Failed to extract responsibleId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<ResponsibleEvent>(ResponsibleQuery.GetQuery(responsibleId),
                responsibleId.ToString()));
    }


    public async Task<string?> CreateStockMessage(string message)
    {
        if (!long.TryParse(message, out var stockId))
        {
            throw new Exception($"Failed to extract stockId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<StockEvent>(StockQuery.GetQuery(stockId),
                stockId.ToString()));
    }

    public async Task<string?> CreateSwcrAttachmentMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract or parse guid SwcrAttachment from message {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<SwcrAttachmentEvent>(
                SwcrAttachmentQuery.GetQuery(message), message));
    }

    public async Task<string?> CreateSwcrMessage(string message) =>
        long.TryParse(message, out var swcrId)
            ? JsonSerializer.Serialize(
                await _eventRepository.QuerySingle<SwcrEvent>(SwcrQuery.GetQuery(swcrId),
                    swcrId.ToString()), DefaultSerializerHelper.SerializerOptions)
            : throw new Exception($"Failed to extract swcrId from message: {message}");

    public async Task<string?> CreateSwcrOtherReferenceMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception(
                $"Failed to extract or parse guid SwcrOtherReferences from message {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<SwcrOtherReferenceEvent>(
                SwcrOtherReferenceQuery.GetQuery(message), message));

    }

    public async Task<string?> CreateSwcrSignatureMessage(string message)
    {
        if (!long.TryParse(message, out var swcrSignatureId))
        {
            throw new Exception($"Failed to extract swcrSignatureId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<SwcrSignatureEvent>(
                SwcrSignatureQuery.GetQuery(swcrSignatureId), message));
    }

    public async Task<string?> CreateSwcrTypeMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract or parse guid SwcrType from message {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<SwcrTypeEvent>(
                SwcrTypeQuery.GetQuery(message), message));
    }
    
    public async Task<string?> CreateTagDocumentMessage(string busEventMessage)
    {
        if (!Guid.TryParse(busEventMessage, out _))
        {
            throw new Exception($"Failed to extract or parse guid TagDocument from message {busEventMessage}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<TagDocumentEvent>(TagDocumentQuery.GetQuery(busEventMessage),
                busEventMessage));
    }

    public async Task<string?> CreateTagEquipmentMessage(string busEventMessage)
    {
        if (!Guid.TryParse(busEventMessage, out _))
        {
            throw new Exception($"Failed to extract or parse guid TagEquipment from message {busEventMessage}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<TagEquipmentEvent>(TagEquipmentQuery.GetQuery(busEventMessage),
                busEventMessage));
    }

    public async Task<string?> CreateTaskMessage(string message) =>
        long.TryParse(message, out var taskId)
            ? JsonSerializer.Serialize(
                await _eventRepository.QuerySingle<TaskEvent>(TaskQuery.GetQuery(taskId), taskId.ToString()))
            : throw new Exception($"Failed to extract checklistId from message: {message}");

    public async Task<string?> CreateWorkOrderChecklistMessage(string message)
    {
        if (!CanGetTwoIdsFromMessage(message.Split(","), out var tagCheckId, out var woId))
        {
            throw new Exception($"Failed to extract Wo or Checklist Id from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<WorkOrderChecklistEvent>(
                WorkOrderChecklistQuery.GetQuery(tagCheckId, woId), message));
    }

    public async Task<string?> CreateWorkOrderCutoffMessage(string message)
    {
        var woInfo = message.Split(","); // woId,cutoffWeek
        if (woInfo.Length != 2)
        {
            throw new Exception($"Failed to extract workOrderId and cutoff week from message: {message}");
        }

        return long.TryParse(woInfo[0], out var workOrderId)
            ? JsonSerializer.Serialize(
                await _eventRepository.QuerySingle<WorkOrderCutoffEvent>(
                    WorkOrderCutoffQuery.GetQuery(workOrderId, woInfo[1]), message),
                DefaultSerializerHelper.SerializerOptions)
            : throw new Exception($"Failed to extract workOrderId from message: {message}");
    }

    public async Task<string?> CreateWorkOrderMaterialMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract workOrderId from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<WorkOrderMaterialEvent>(
                WorkOrderMaterialQuery.GetQuery(message), message), DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateWorkOrderMessage(string message)
    {
        if (!long.TryParse(message, out var workOrderId))
        {
            throw new Exception($"Failed to extract guid from message {message}");
        }

        var queryString = WorkOrderQuery.GetQuery(workOrderId);
        var workOrderEvent = await _eventRepository.QuerySingle<WorkOrderEvent>(queryString, message);
        return JsonSerializer.Serialize(workOrderEvent, DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateWorkOrderMilestoneMessage(string message)
    {
        if (!CanGetTwoIdsFromMessage(message.Split(","), out var workOrderId, out var milestoneId))
        {
            throw new Exception($"Failed to extract ids from message {message}");
        }

        var queryString = WorkOrderMilestoneQuery.GetQuery(workOrderId, milestoneId);
        var workOrderMilestoneEvent =
            await _eventRepository.QuerySingle<WorkOrderMilestoneEvent>(queryString, message);
        return JsonSerializer.Serialize(workOrderMilestoneEvent, DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateHeatTracePipeTestMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract or parse guid HeatTracePipeTest from message {message}");
        }

        var queryStringAndParams = HeatTracePipeTestQuery.GetQuery(message);
        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<HeatTracePipeTestEvent>(
                queryStringAndParams, message), DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateNotificationMessage(string message)
    {
        if (!long.TryParse(message, out var documentId))
        {
            throw new Exception($"Failed to extract documentId from message: {message}");
        }

        var queryStringAndParams = NotificationQuery.GetQuery(documentId);
        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<NotificationEvent>(
                queryStringAndParams, message), DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateNotificationWorkOrderMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract or parse guid NotificationWorkOrder from message {message}");
        }

        var queryStringAndParams = NotificationWorkOrderQuery.GetQuery(message);
        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<NotificationWorkOrderEvent>(
                queryStringAndParams, message), DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreateNotificationCommPkgMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract or parse guid NotificationCommPkg from message {message}");
        }

        //Try to find commpkg reference of type 'Other' first
        var queryStringAndParamsOther = NotificationCommPkgOtherQuery.GetQuery(message);
        var commPkgEvent = await _eventRepository.QuerySingle<NotificationCommPkgEvent>(queryStringAndParamsOther, message);

        if (commPkgEvent is null)
        {
            //Try to find commpkg reference of type 'Boundary'
            var queryStringAndParamsBoundary = NotificationCommPkgBoundaryQuery.GetQuery(message);
            commPkgEvent = await _eventRepository.QuerySingle<NotificationCommPkgEvent>(queryStringAndParamsBoundary, message);
        }

        return JsonSerializer.Serialize(commPkgEvent, DefaultSerializerHelper.SerializerOptions);
    }


    public async Task<string?> CreateNotificationSignatureMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract or parse guid NotificationSignature from message {message}");
        }

        var queryStringAndParams = NotificationSignatureQuery.GetQuery(message);
        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<NotificationSignatureEvent>(
                queryStringAndParams, message), DefaultSerializerHelper.SerializerOptions);
    }

    public async Task<string?> CreatePunchPriorityLibRelationMessage(string message)
    {
        if (!Guid.TryParse(message, out _))
        {
            throw new Exception($"Failed to extract libToLibRelation guid from message: {message}");
        }

        return JsonSerializer.Serialize(
            await _eventRepository.QuerySingle<PunchPriorityLibRelationEvent>(
                PunchPriorityLibraryRelationQuery.GetQuery(message), message), DefaultSerializerHelper.SerializerOptions);
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
}
