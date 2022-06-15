using System;
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
    private readonly ITagDetailsRepository _tagDetailsRepository;
    private readonly IDocumentRepository _documentRepository;
    private readonly IWorkOrderRepository _workOrderRepository;
    private readonly Regex _rx = new(@"[\a\e\f\n\r\t\v]", RegexOptions.Compiled);

    public BusEventService(ITagDetailsRepository tagDetailsRepository,
        IDocumentRepository documentRepository, IWorkOrderRepository workOrderRepository)
    {
        _tagDetailsRepository = tagDetailsRepository;
        _documentRepository = documentRepository;
        _workOrderRepository = workOrderRepository;
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

    public async Task<string> CreateQueryMessage(string busEventMessage)
    {
        if (!long.TryParse(busEventMessage, out var documentId))
        {
            throw new Exception("Failed to extract documentId from message");
        }

        return WashString(await _documentRepository.GetQueryMessage(documentId));
    }

    public async Task<string> CreateDocumentMessage(string busEventMessage)
    {
        if (!long.TryParse(busEventMessage, out var documentId))
        {
            throw new Exception("Failed to extract documentId from message");
        }

        return WashString(await _documentRepository.GetDocumentMessage(documentId));
    }

    public async Task<string> CreateWorkOrderMessage(string busEventMessage)
    {
        if (!long.TryParse(busEventMessage, out var workOrderId))
        {
            throw new Exception("Failed to extract workOrderId from message");
        }

        return WashString(await _workOrderRepository.GetWorkOrderMessage(workOrderId));
    }

    public bool IsNotLatestMaterialEvent(IEnumerable<BusEvent> events, BusEvent busEvent)
    {
        var compareTo = JsonSerializer.Deserialize<WoMaterialIdentifier>(WashString(busEvent.Message));

        return events.Where(e => e.Event== WoMaterialTopic.TopicName).Any(e =>
        {
            var woMaterial = JsonSerializer.Deserialize<WoMaterialIdentifier>(WashString(e.Message));
            return woMaterial != null
                   && compareTo != null
                   && woMaterial.ItemNo == compareTo.ItemNo && woMaterial.WoId == compareTo.WoId
                   && e.Created > busEvent.Created;
        });
    }

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


}
