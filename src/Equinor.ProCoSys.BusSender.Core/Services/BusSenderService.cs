﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.BusSenderWorker.Core.Extensions;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class BusSenderService : IBusSenderService
{
    private readonly IPcsBusSender _pcsBusSender;
    private readonly IBusEventRepository _busEventRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<BusSenderService> _logger;
    private readonly ITelemetryClient _telemetryClient;
    private readonly IBusEventService _service;
    private readonly Stopwatch _sw;

    public BusSenderService(IPcsBusSender pcsBusSender,
        IBusEventRepository busEventRepository,
        IUnitOfWork unitOfWork,
        ILogger<BusSenderService> logger,
        ITelemetryClient telemetryClient,
        IBusEventService service)
    {
        _pcsBusSender = pcsBusSender;
        _busEventRepository = busEventRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
        _telemetryClient = telemetryClient;
        _service = service;
        _sw = new Stopwatch();
    }

    public async Task HandleBusEvents()
    {
        try
        {
            _sw.Start();
            var events = await _busEventRepository.GetEarliestUnProcessedEventChunk();
            if (events.Any())
            {
                _logger.LogInformation("BusSenderService found {count} messages to process after {sw} ms", events.Count, _sw.ElapsedMilliseconds);
                _telemetryClient.TrackMetric("BusSender Chunk", events.Count);
               _logger.LogDebug("First Track after {ms} ms", _sw.ElapsedMilliseconds);

                await ProcessBusEvents(events);
                _logger.LogInformation("BusSenderService ProcessBusEvents used {sw} ms", _sw.ElapsedMilliseconds);

            }
            _sw.Reset();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "BusSenderService execute send failed");
            throw;
        }

        _logger.LogInformation("BusSenderService DoWorkerJob finished ");
        _telemetryClient.Flush();
    }

    public async Task CloseConnections()
    {
        _logger.LogInformation("BusSenderService stop reader and close all connections");
        await _pcsBusSender.CloseAllAsync();
    }

    private List<BusEvent> SetDuplicatesToSkipped(List<BusEvent> events)
    {
        var duplicateGroupings = FilterOnSimpleMessagesAndGroupDuplicates(events);
        var amount = 0; //TODO remove, when not needed anymore
        foreach (var group in duplicateGroupings)
        {
            amount += group.Count() - 1;
            
            SetAllButOneEventToSkipped(group);
        }
        _logger.LogDebug("skipped {amount} of messages", amount);
        return events;
    }
    private static void SetAllButOneEventToSkipped(IEnumerable<BusEvent> group)
    {
        foreach (var busEvent in group.SkipLast(1))
        {
            busEvent.Status = Status.Skipped;
        }
    }

    /// <summary>
    ///     Removes all events that have Message = more than just a simple Id that can be converted to a long.
    ///     Then groups the events into bulks based on Id and Event type.
    /// </summary>
    /// <param name="events"></param>
    /// <returns></returns>
    private static IEnumerable<IGrouping<(string, long), BusEvent>> FilterOnSimpleMessagesAndGroupDuplicates(
        IEnumerable<BusEvent> events)
        => events.Where(e => long.TryParse(e.Message, out var id))
            .GroupBy(e => (e.Event, long.Parse(e.Message)));

    private async Task ProcessBusEvents(List<BusEvent> events)
    {
        events = SetDuplicatesToSkipped(events);
        var dsw = Stopwatch.StartNew();

        await events.Where(busEvent => busEvent.Status == Status.UnProcessed)
            .ForEachAsync(20, async e => await UpdateEventBasedOnTopic(e));
       _logger.LogDebug("Update loop finished at at {sw} ms", dsw.ElapsedMilliseconds);
        //
        // var listOfTasks = events.Where(busEvent => busEvent.Status == Status.UnProcessed)
        //     // .AsParallel().
        //     // .AsOrdered()
        //     .Select(async e => await UpdateEventBasedOnTopic(e))
        //     .ToList();

        // await Task.WhenAll(listOfTasks);
        // _logger.LogDebug("Updated events at {sw} ms",  dsw.ElapsedMilliseconds);

        await _unitOfWork.SaveChangesAsync(); 
        _logger.LogDebug("After First save {sw} ms",  dsw.ElapsedMilliseconds);


        // /***
        // * group messages into batches, and fail before sending if message exceeds size limit
        // *
        // * Pattern taken from:
        // * https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/servicebus/Azure.Messaging.ServiceBus/MigrationGuide.md
        // */
        //
        // Queue<BusEvent> messages = new();
        // events.ForEach(e => messages.Enqueue(e));
        // var messageCount = messages.Count;
        //
        // while (messages.Count > 0)
        // {
        //     using var messageBatch = await _pcsBusSender.CreateMessageBatchAsync();
        //     // add first unsent message to batch
        //     if (messageBatch.TryAddMessage(new ServiceBusMessage(messages.Peek().MessageToSend ?? messages.Peek().Message)))
        //     {
        //        var m = messages.Dequeue();
        //         TrackMessage(m);
        //     }
        //     else
        //     {
        //         // if the first message can't fit, then it is too large for the batch
        //         throw new Exception($"Message {messageCount - messages.Count} is too large and cannot be sent.");
        //     }
        //
        //     // add as many messages as possible to the current batch
        //     while (messages.Count > 0 && messageBatch.TryAddMessage(new ServiceBusMessage(messages.Peek().MessageToSend ?? messages.Peek().Message)))
        //     {
        //         var m = messages.Dequeue();
        //         TrackMessage(m);
        //     }
        //     await _pcsBusSender.SendMessagesAsync(messageBatch);
        //     
        // }
        //
        //
        // private void TrackMessage(BusEvent busEvent)
        // {
        //     var busEventMessageToSend = busEvent.MessageToSend ?? busEvent.Message;
        //     var message = JsonSerializer.Deserialize<BusEventMessage>(_service.WashString(busEventMessageToSend));
        //     if (message != null && string.IsNullOrEmpty(message.ProjectName))
        //     {
        //         message.ProjectName = "_";
        //     }
        //
        //     TrackMetric(message);
        //
        // }

        foreach (var busEvent in events.Where(busEvent => busEvent.Status == Status.UnProcessed))
        {
            var busEventMessageToSend = busEvent.MessageToSend ?? busEvent.Message;
            var message = JsonSerializer.Deserialize<BusEventMessage>(_service.WashString(busEventMessageToSend));
            if (message != null && string.IsNullOrEmpty(message.ProjectName))
            {
                message.ProjectName = "_";
            }

            TrackMetric(message);

            await _pcsBusSender.SendAsync(busEvent.Event, _service.WashString(busEventMessageToSend));
            _logger.LogDebug("send takes {sw} ms", dsw.ElapsedMilliseconds);
            
            TrackEvent(busEvent.Event, message);
            
            dsw.Restart();
            busEvent.Status = Status.Sent;
            await _unitOfWork.SaveChangesAsync();
            _logger.LogDebug("Save in batch takes {sw} ms", dsw.ElapsedMilliseconds);
        }
    }

    private static bool HasUnsavedChanges(IEnumerable<BusEvent> events) => events.Any(e => e.Status != Status.UnProcessed);

    private async Task<BusEvent> UpdateEventBasedOnTopic(BusEvent busEvent)
    {
        _logger.LogDebug("Started update for {event}", busEvent.Event);
        var sw = Stopwatch.StartNew();
        switch (busEvent.Event)
        {
            case TagTopic.TopicName:
                {
                    busEvent.Message = await _service.AttachTagDetails(busEvent.Message);
                    break;
                }
            case ChecklistTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateChecklistMessage);
                    break;
                }
            case CommPkgQueryTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateCommPkgQueryMessage);
                    break;
                }
            case QueryTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateQueryMessage);
                    break;
                }
            case QuerySignatureTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateQuerySignatureMessage);
                    break;
                }
            case DocumentTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateDocumentMessage);
                    break;
                }
            case MilestoneTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateMilestoneMessage);
                    break;
                }
            case WorkOrderTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateWorkOrderMessage);

                    break;
                }
            case WorkOrderCutoffTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateWorkOrderCutOffMessage);
                    break;
                }
            case CallOffTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateCallOffMessage);
                    break;
                }
            case WoChecklistTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateWoChecklistMessage);
                    break;
                }
            case WoMilestoneTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateWoMilestoneMessage);
                    break;
                }
            case SwcrTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateSwcrMessage);
                    break;
                }
            case SwcrSignatureTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateSwcrSignatureMessage);
                    break;
                }
            case LoopContentTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateLoopContentMessage);
                    break;
                }
            case StockTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateStockMessage);
                    break;
                }
            case PipingRevisionTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreatePipingRevisionMessage);
                    break;
                }
            case PipingSpoolTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreatePipingSpoolMessage);
                    break;
                }
            case WoMaterialTopic.TopicName:
                {
                    await CreateAndSetMessage(busEvent, _service.CreateWoMaterialMessage);
                    break;
                }
        }

        _logger.LogDebug("Update for  {event} took {ms}", busEvent.Event, sw.ElapsedMilliseconds);
        sw.Stop();
        return busEvent;
    }

    /***
     * Takes a function to create a message, caller needs to make sure that event has the correct topic for the function.
     */
    private static async Task CreateAndSetMessage(BusEvent busEvent, Func<string, Task<string>> createMessageFunction)
    {
        var message = await createMessageFunction(busEvent.Message);
        if (message == null)
        {
            busEvent.Status = Status.NotFound;
        }
        else
        {
            busEvent.MessageToSend = message;
        }
    }

    private void TrackMetric(BusEventMessage message) =>
        _telemetryClient.TrackMetric("BusSender Topic", 1, "Plant", "ProjectName", message?.Plant?[4..],
            message?.ProjectName?.Replace('$', '_') ?? "NoProject");

    private void TrackEvent(string eventType, BusEventMessage message)
    {
        var properties = new Dictionary<string, string>
        {
            { "Event", eventType },
            { "Plant", message.Plant[4..] },
            { "ProjectName", message.ProjectName?.Replace('$', '_') }
        };
        if (!string.IsNullOrWhiteSpace(message.McPkgNo))
        {
            properties.Add("McPkgNo", message.McPkgNo);
        }

        if (!string.IsNullOrWhiteSpace(message.CommPkgNo))
        {
            properties.Add("CommPkgNo", message.CommPkgNo);
        }

        _telemetryClient.TrackEvent("BusSender Send",
            properties);
    }




}
