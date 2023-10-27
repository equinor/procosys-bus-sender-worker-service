using System.Diagnostics;
using Equinor.ProCoSys.PcsServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

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
            b.AddAzureStorageQueues();
            b.AddAzureStorageBlobs();
        });

        builder.UseContentRoot(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName))
            .ConfigureServices((hostContext, services) =>
            {
                services.AddAzureAppConfiguration();
                services.AddScoped<IBusReceiverService, BusReceiverService>();
                services.AddSingleton<IBusReceiverServiceFactory, ScopedBusReceiverServiceFactory>();
                services.AddPcsServiceBusIntegration(options => options
                    .UseBusConnection(hostContext.Configuration["ServiceBusConnectionString"])
                    .WithRenewLeaseInterval(4000)
                    .WithSubscription(PcsTopicConstants.Tag, "test_tag")
                );

                services.BuildServiceProvider();
            });
        return builder;
    }

    private static void OnProcessExit(ILogger? logger)
        => logger?.LogInformation("Sync stopped at: {Now}", DateTimeOffset.Now);
}
