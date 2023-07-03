﻿using System.Diagnostics;
using Equinor.ProCoSys.BusReceiver.Consumers;
using Equinor.ProCoSys.Completion.MessageContracts.Punch;
using Equinor.ProCoSys.PcsServiceBus.Receiver;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using MassTransit;

namespace Equinor.ProCoSys.BusReceiver;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        ILogger? logger = host.Services.GetService<ILogger<Program>>();
        AppDomain.CurrentDomain.ProcessExit += (_, _) => OnProcessExit(logger);
        await host.RunAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.Build();
            });

        builder.ConfigureWebJobs(b =>
        {
            b.AddAzureStorageCoreServices();
            b.AddAzureStorageQueues();
            b.AddAzureStorageBlobs();
        });

        builder.UseContentRoot(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName))
            .ConfigureServices((hostContext, services) =>
            {
                services.AddAzureAppConfiguration();
                services.AddScoped<IBusReceiverService, BusReceiverService>();
                services.AddSingleton<IBusReceiverServiceFactory, ScopedBusReceiverServiceFactory>();

                services.AddMassTransit(x =>
                {
                    x.AddConsumer<PunchCreatedConsumer>();
                    x.UsingAzureServiceBus((context, cfg) =>
                    {
                        var connectionString = hostContext.Configuration["ServiceBusConnectionString"];
                        cfg.Host(connectionString);
                        
                        cfg.Message<IPunchCreatedV1>(topologyConf 
                             => topologyConf.SetEntityName("punch-created"));

                        cfg.SubscriptionEndpoint<IPunchCreatedV1>("pcs-main",e =>
                        {
                            e.Consumer<PunchCreatedConsumer>(context);
                            e.RequiresSession = true;
                        });

                    });
                });
                
                services.BuildServiceProvider();
            });
        return builder;
    }

    private static void OnProcessExit(ILogger? logger)
        => logger?.LogInformation("Sync stopped at: {Now}", DateTimeOffset.Now);
}
