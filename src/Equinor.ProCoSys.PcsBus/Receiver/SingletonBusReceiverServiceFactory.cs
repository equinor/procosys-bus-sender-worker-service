using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver
{
    public class SingletonBusReceiverServiceFactory : IBusReceiverServiceFactory
    {
        private readonly IBusReceiverService _service;

        public SingletonBusReceiverServiceFactory(IBusReceiverService service) => _service = service;

        public IBusReceiverService GetServiceInstance() => _service;
    }
}
