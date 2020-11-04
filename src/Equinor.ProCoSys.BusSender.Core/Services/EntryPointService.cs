using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Core.Interfaces;

namespace Equinor.ProCoSys.BusSender.Core.Services
{
    public class EntryPointService : IEntryPointService
    {
        private readonly IServiceLocator _serviceLocator;

        public EntryPointService(IServiceLocator serviceLocator) => _serviceLocator = serviceLocator;

        public async Task SendMessageChunk()
        {
            var service = _serviceLocator.GetService<IBusSenderService>();
            await service.SendMessageChunk();
        }

        public async Task StopService()
        {
            var service = _serviceLocator.GetService<IBusSenderService>();
            await service.StopService();
        }
    }
}
