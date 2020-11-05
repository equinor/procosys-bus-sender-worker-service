using System;

namespace Equinor.ProCoSys.BusSender.Core.Interfaces
{
    public interface IServiceLocator : IDisposable
    {
        T GetService<T>();
    }
}
