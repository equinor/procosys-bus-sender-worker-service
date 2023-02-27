
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

namespace Equinor.ProCoSys.BusReceiver;

public class BusReceiverService : IBusReceiverService
{
    public  Task ProcessMessageAsync(string pcsTopic, string message, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    
}
