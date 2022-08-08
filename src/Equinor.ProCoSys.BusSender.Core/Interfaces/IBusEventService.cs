﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;

public interface IBusEventService
{
    bool IsNotLatestMaterialEvent(IEnumerable<BusEvent> events, BusEvent busEvent);
    Task<string> AttachTagDetails(string tagMessage);
    string WashString(string message);
    Task<string> CreateQueryMessage(string busEventMessage);
    Task<string> CreateQuerySignatureMessage(string busEventMessage);
    Task<string> CreateDocumentMessage(string busEventMessage);
    Task<string> CreateWorkOrderMessage(string busEventMessage);
    Task<string> CreateChecklistMessage(string busEventMessage);
    Task<string> CreateCallOffMessage(string busEventMessage);
    Task<string> CreateWoChecklistMessage(string busEventMessage);
    Task<string> CreateWoMilestoneMessage(string message);
    Task<string> CreateWoMaterialMessage(string busEventMessage);
    Task<string> CreateStockMessage(string busEventMessage);
    Task<string> CreateSwcrMessage(string busEventMessage);
    Task<string> CreateSwcrSignatureTaskMessage(string busEventMessage);
    Task<string> CreatePipingRevisionMessage(string busEventMessage);
    Task<string> CreatePipingSpoolMessage(string busEventMessage);
    Task<string> CreateLoopContentMessage(string busEventMessage);
}
