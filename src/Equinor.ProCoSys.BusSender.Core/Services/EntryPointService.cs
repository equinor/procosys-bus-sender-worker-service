using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class EntryPointService : IEntryPointService
{
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;

    public EntryPointService(IServiceProvider services, IConfiguration configuration)
    {
        _services = services;
        _configuration = configuration;
    }

    public async Task<bool> DoWorkerJob()
    {
        using var scope = _services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBusSenderService>();
        var multiInstanceSupport = bool.Parse(_configuration["MultiInstanceSupport"]??"false");
        if (multiInstanceSupport)
        {
            await service.HandleBusEvents();
        }
        else
        {
            await service.HandleBusEventsSingleInstance();
        }
        return service.HasPendingEventsForCurrentPlant();
    }

    public async Task StopService()
    {
        using var scope = _services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBusSenderService>();
        await service.CloseConnections();
    }
}
