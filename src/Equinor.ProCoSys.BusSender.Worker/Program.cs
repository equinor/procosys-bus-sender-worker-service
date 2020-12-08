using Equinor.ProCoSys.BusSender.Infrastructure;
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
                .ConfigureLogging((hostingContext, builder) =>
                {
                    builder.AddFile("c:\\Logs\\myapp.txt");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext(hostContext.Configuration["ConnectionString"]);
                    services.AddTopicClients(
                        hostContext.Configuration["ServiceBusConnectionString"],
                        hostContext.Configuration["TopicNames"]);
                    services.AddRepositories();
                    services.AddServices();
                    services.AddApplicationInsightsTelemetryWorkerService();

                    services.AddHostedService<TimedWorkerService>();
                });
    }
}
