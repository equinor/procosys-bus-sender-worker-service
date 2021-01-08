using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.BusSender.Core.Services
{
    public class EntryPointService : IEntryPointService
    {
        private readonly IServiceLocator _serviceLocator;

        public EntryPointService(IServiceLocator serviceLocator) => _serviceLocator = serviceLocator;

        public async Task SendMessageChunk()
        {
            using var scope = _serviceLocator.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBusSenderService>();
            await service.SendMessageChunk();
        }

        public async Task StopService()
        {
            using var scope = _serviceLocator.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IBusSenderService>();
            await service.StopService();
        }
    }
}
