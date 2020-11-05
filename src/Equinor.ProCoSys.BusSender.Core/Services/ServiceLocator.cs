using Equinor.ProCoSys.BusSender.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.BusSender.Core.Services
{
    public sealed class ServiceLocator : IServiceLocator
    {
        private readonly IServiceScopeFactory _factory;
        private IServiceScope _scope;

        public ServiceLocator(IServiceScopeFactory factory) => _factory = factory;

        public T GetService<T>()
        {
            _scope ??= _factory.CreateScope();
            return _scope.ServiceProvider.GetService<T>();
        }

        public void Dispose()
        {
            _scope?.Dispose();
            _scope = null;
        }
    }
}
