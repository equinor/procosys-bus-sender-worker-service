
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

namespace Equinor.ProCoSys.BusReceiver;

public class BusReceiverService : IBusReceiverService
{
    public async Task ProcessMessageAsync(PcsTopic pcsTopic, string message, CancellationToken token)
    {

        await DoSmt();


        throw new NotImplementedException();
    }

    private async Task DoSmt()
    {
        throw new NotImplementedException();
    }
}
