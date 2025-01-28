using System;
using System.Collections.Generic;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Validation;
using Equinor.ProCoSys.PcsServiceBus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure;

public static class ServiceCollectionSetup
{
    /**
     * Maximum open cursors in the Pcs database is configured to 300 as per 05.03.2020.
     * When doing batch updates/inserts, oracle opens a cursor per update/insert to keep track of
     * the amount of entities updated. The default seems to be 200, but we're setting it explicitly anyway
     * in case the default changes in the future. This is to avoid ORA-01000: maximum open cursors exceeded.
     */
    private const int MaxOpenCursors = 200;

    private static readonly LoggerFactory LoggerFactory =
        new(new[] { new DebugLoggerProvider() });

    public static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString)
        => services.AddDbContext<BusSenderServiceContext>(options =>
        {
            options.UseOracle(connectionString, b => b.MaxBatchSize(MaxOpenCursors));
            options.UseLoggerFactory(LoggerFactory);
            options.EnableSensitiveDataLogging();
        }).AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<BusSenderServiceContext>());

    public static IServiceCollection AddRepositories(this IServiceCollection services)
        => services.AddScoped<IPlantRepository, PlantRepository>()
            .AddScoped<IBusEventRepository, BusEventRepository>()
            .AddScoped<ITagDetailsRepository, TagDetailsRepository>();

    public static IServiceCollection AddServices(this IServiceCollection services)
        => services.AddSingleton<IEntryPointService, EntryPointService>()
            .AddSingleton<IPlantService, PlantService>()
            .AddScoped<ITelemetryClient, ApplicationInsightsTelemetryClient>()
            .AddScoped<IBusSenderService, BusSenderService>()
            .AddScoped<IBusEventService, BusEventService>()
            .AddScoped<IQueueMonitorService, QueueMonitorService>()
            .AddScoped<ISystemClock, TimeService>()
            .AddScoped<IEventRepository, EventRepository>();

    public static IServiceCollection AddInstanceConfig(this IServiceCollection services, IConfiguration configuration) => 
        services.AddSingleton<InstanceConfig>(serviceProvider =>
       {
           var config = new InstanceConfig();
           var instanceOptions = serviceProvider.GetRequiredService<IOptions<InstanceOptions>>();
           var plantsByInstances = configuration.GetRequiredSection("PlantsByInstance").Get<List<PlantsByInstance>>();

           ConfigurationValidator.ValidatePlantsByInstance(plantsByInstances);
           ConfigurationValidator.ValidateInstanceOptions(instanceOptions.Value);

           var plantService = serviceProvider.GetRequiredService<IPlantService>();

           // Use Lazy to ensure GetAllPlants is called only once
           var lazyAllPlants = new Lazy<List<string>>(() =>
           {
               using (var scope = serviceProvider.CreateScope())
               {
                   var plantRepository = scope.ServiceProvider.GetRequiredService<IPlantRepository>();
                   return plantRepository.GetAllPlants();
               }
           });

           var plants = plantService.GetPlantsHandledByInstance(plantsByInstances, lazyAllPlants.Value, instanceOptions.Value.InstanceName);
           config.PlantsHandledByCurrentInstance = plants;

           return config;
       });
}
