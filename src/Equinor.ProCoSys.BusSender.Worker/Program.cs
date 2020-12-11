using System;
using System.Diagnostics;
using System.IO;
using Equinor.ProCoSys.BusSender.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSender.Worker
{
    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((_, config) =>
                {
                    config = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                })
                .UseContentRoot(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
                .ConfigureLogging(builder =>
                {
                    builder.AddApplicationInsights();
                    builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
                                 ("", LogLevel.Information);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.AddDbContext(hostContext.Configuration["ConnectionString"]);
                    services.AddTopicClients(
                        hostContext.Configuration["ServiceBusConnectionString"],
                        hostContext.Configuration["TopicNames"]);
                    services.AddRepositories();
                    services.AddServices();

                    services.AddHostedService<TimedWorkerService>();
                });
    }
}
