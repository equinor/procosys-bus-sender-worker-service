using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Data;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;
using Equinor.ProCoSys.PcsServiceBus.Sender;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure;

public static class ServiceCollectionSetup
{
    /**
         * Maximum open cursors in the Pcs database is configured to 300 as per 05.03.2020.
         * When doing batch updates/inserts, oracle opens a cursor per update/insert to keep track of
         * the amount of entities updated. The default seems to be 200, but we're setting it explicitly anyway
         * in case the default changes in the future. This is to avoid ORA-01000: maximum open cursors exceeded.
         **/
    private const int MAX_OPEN_CURSORS = 200;

    public static readonly LoggerFactory LoggerFactory =
        new LoggerFactory(new[]
        {
            new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
        });

    public static IServiceCollection AddDbContext(this IServiceCollection services, string connectionString)
        => services.AddDbContext<BusSenderServiceContext>(options =>
        {
            options.UseOracle(connectionString, b => b.MaxBatchSize(MAX_OPEN_CURSORS));
            options.UseLoggerFactory(LoggerFactory);
            options.EnableSensitiveDataLogging();
        }).AddScoped<IUnitOfWork>(serviceProvider => serviceProvider.GetRequiredService<BusSenderServiceContext>());

    public static async void AddTopicClients(this IServiceCollection services, string serviceBusConnectionString, string topicNames)
    {
        var topics = topicNames.Split(',');
        var pcsBusSender = new PcsBusSender();
        var options = new ServiceBusClientOptions { EnableCrossEntityTransactions = true };
        await using var client = new ServiceBusClient(serviceBusConnectionString, options);
        foreach (var topicName in topics)
        {
            var serviceBusSender = client.CreateSender(topicName);
            pcsBusSender.Add(topicName, serviceBusSender);
        }

        services.AddSingleton<IPcsBusSender>(pcsBusSender);
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
        => services.AddScoped<IBusEventRepository, BusEventRepository>()
            .AddScoped<ITagDetailsRepository, TagDetailsRepository>()
            .AddScoped<IBusSenderMessageRepository,BusSenderMessageRepository>();

    public static IServiceCollection AddServices(this IServiceCollection services)
        => services.AddSingleton<IEntryPointService, EntryPointService>()
            .AddScoped<ITelemetryClient, ApplicationInsightsTelemetryClient>()
            .AddScoped<IBusSenderService, BusSenderService>()
            .AddScoped<IBusEventService,BusEventService>();
}
