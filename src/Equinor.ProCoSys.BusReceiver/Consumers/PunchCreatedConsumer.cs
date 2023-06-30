using Equinor.ProCoSys.Completion.MessageContracts.Punch;
using MassTransit;

namespace Equinor.ProCoSys.BusReceiver.Consumers;

public class PunchCreatedConsumer : IConsumer<IPunchCreatedV1>
{
    public Task Consume(ConsumeContext<IPunchCreatedV1> context)
    {
        
        var message = context.Message;

       // throw new Exception();
        // Process the message here...
        return Task.CompletedTask;
    }
}


