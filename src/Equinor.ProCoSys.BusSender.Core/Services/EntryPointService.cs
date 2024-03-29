﻿using System;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Services;

public class EntryPointService : IEntryPointService
{
    private readonly IServiceProvider _services;

    public EntryPointService(IServiceProvider services) => _services = services;

    public async Task DoWorkerJob()
    {
        using var scope = _services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBusSenderService>();
        await service.HandleBusEvents();
    }

    public async Task StopService()
    {
        using var scope = _services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IBusSenderService>();
        await service.CloseConnections();
    }
}
