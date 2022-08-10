﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.PcsServiceBus.Topics;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class BusEventService : IBusEventService
{
    private readonly IBusSenderMessageRepository _busSenderMessageRepository;
    private readonly Regex _rx = new(@"[\a\e\f\n\r\t\v]", RegexOptions.Compiled);
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

    public async Task<string> CreateDocumentMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var documentId)
            ? WashString(await _busSenderMessageRepository.GetDocumentMessage(documentId))
            : throw new Exception("Failed to extract documentId from message");

    public async Task<string> CreateLoopContentMessage(string busEventMessage)
        => long.TryParse(busEventMessage, out var loopContentId)
            ? WashString(await _busSenderMessageRepository.GetLoopContentMessage(loopContentId))
            : throw new Exception("Failed to extract LoopContent from message");

    public async Task<string> CreatePipingRevisionMessage(string busEventMessage)
        => long.TryParse(busEventMessage, out var pipingRevisionId)
            ? WashString(await _busSenderMessageRepository.GetPipingRevisionMessage(pipingRevisionId))
            : throw new Exception("Failed to extract PipeRevision from message");

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
            : throw new Exception("Failed to extract Wo xor Checklist Id from message");

    public async Task<string> CreateWoMaterialMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var workOrderId)
            ? WashString(await _busSenderMessageRepository.GetWorkOrderMaterialMessage(workOrderId))
            : throw new Exception("Failed to extract workOrderId from message");

    public async Task<string> CreateWoMilestoneMessage(string message) =>
        CanGetTwoIdsFromMessage(message.Split(","), out var workOrderId, out var milestoneId)
            ? WashString(await _busSenderMessageRepository.GetWorkOrderMilestoneMessage(workOrderId, milestoneId))
            : throw new Exception("Failed to extract WorkOrder xor Milestone Id from message");

    public async Task<string> CreateWorkOrderMessage(string busEventMessage) =>
        long.TryParse(busEventMessage, out var workOrderId)
            ? WashString(await _busSenderMessageRepository.GetWorkOrderMessage(workOrderId))
            : throw new Exception("Failed to extract workOrderId from message");

    public string WashString(string busEventMessage)
    {
        if (string.IsNullOrEmpty(busEventMessage))
        {
            return busEventMessage;
        }

        busEventMessage = busEventMessage.Replace("\r", "");
        busEventMessage = busEventMessage.Replace("\n", "");
        busEventMessage = busEventMessage.Replace("\t", "");
        busEventMessage = busEventMessage.Replace("\f", "");
        busEventMessage = _rx.Replace(busEventMessage, m => Regex.Escape(m.Value));

        ////Removes non printable characters
        const string Pattern = "[^ -~]+";
        var regExp = new Regex(Pattern);
        busEventMessage = regExp.Replace(busEventMessage, "");

        return busEventMessage;
    }

    private static bool CanGetTwoIdsFromMessage(IReadOnlyList<string> array, out long id1, out long id2)
    {
        id1 = 0;
        id2 = 0;
        return array.Count == 2
               && long.TryParse(array[0], out id1)
               && long.TryParse(array[1], out id2);
    }
}
