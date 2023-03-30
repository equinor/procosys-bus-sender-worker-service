using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

namespace Equinor.ProCoSys.BusReceiver;

public class BusReceiverService : IBusReceiverService
{
    public  Task ProcessMessageAsync(PcsTopic pcsTopic, string message, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    
}
