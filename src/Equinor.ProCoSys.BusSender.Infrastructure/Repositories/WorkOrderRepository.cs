using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Equinor.ProCoSys.PcsServiceBus.Queries;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;

public class WorkOrderRepository : BusSenderRepository, IWorkOrderRepository
{
    public WorkOrderRepository(BusSenderServiceContext context, ILogger<WorkOrderRepository> logger) 
        : base(context,logger)
    {
    }

    public async Task<string> GetWorkOrderMessage(long workOrderId)
        => await ExecuteQuery(WorkOrderQuery.GetQuery(workOrderId), workOrderId);
}
