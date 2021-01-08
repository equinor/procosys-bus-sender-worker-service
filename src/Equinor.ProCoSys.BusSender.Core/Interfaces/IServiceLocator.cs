using System;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.BusSender.Core.Interfaces
{
    public interface IServiceLocator : IDisposable
    {
        IServiceScope CreateScope();
        T GetService<T>();
    }
}
