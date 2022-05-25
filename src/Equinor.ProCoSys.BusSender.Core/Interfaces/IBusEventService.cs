using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Interfaces
{
    public interface IBusEventService
    {
        bool IsNotLatestMaterialEvent(IEnumerable<BusEvent> events, BusEvent busEvent);
        Task<string> AttachTagDetails(string tagMessage);
        string WashString(string busEventMessage);
        Task<string> CreateQueryMessage(string busEventMessage);
        Task<string> CreateDocumentMessage(string busEventMessage);
    }



}
