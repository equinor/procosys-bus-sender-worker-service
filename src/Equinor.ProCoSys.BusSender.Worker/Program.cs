using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSender.Worker;

public class Program
{
    public IConfiguration Configuration { get; }

    public Program(IConfiguration configuration)
        => Configuration = configuration;

    public static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        ILogger? logger = host.Services.GetService<ILogger<Program>>();
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddUserSecrets<Program>(true);
                var settings = config.Build();

                var azConfig = settings.GetValue<bool>("UseAzureAppConfiguration");
                if (azConfig)
                {
                    config.AddAzureAppConfiguration(options =>
                    {
                        var connectionString = settings["ConnectionStrings:AppConfig"];
                        options.Connect(connectionString)
                            .ConfigureKeyVault(kv =>
                            {
                                kv.SetCredential(new DefaultAzureCredential());
                            })
                            .Select(KeyFilter.Any)
                            .Select(KeyFilter.Any, context.HostingEnvironment.EnvironmentName)
                            .ConfigureRefresh(refreshOptions =>
                            {
                                refreshOptions.Register("Sentinel", true);
                                refreshOptions.SetCacheExpiration(TimeSpan.FromMinutes(5));
                            });
                    });
                }

                else if (settings["EnvironmentName"] == "Local")
                {
                    config.AddAzureAppConfiguration(options =>
                    {

                        options.Connect(settings["AppConfig"])
                            .ConfigureKeyVault(kv =>
                            {
                                kv.SetCredential(new DefaultAzureCredential());
                            })
                            .ConfigureRefresh(opt =>
                            {
                                opt.Register("Sentinel", true);
                                opt.SetCacheExpiration(TimeSpan.FromMinutes(5));
                            })
                            .Select(KeyFilter.Any)
                            .Select(KeyFilter.Any, settings["Azure:AppConfigLabelFilter"]);
                    });
                }
                else
                {
                    config.AddAzureAppConfiguration(options =>
                    {
                        var appConfigurationName = settings["Azure:AppConfig"];
                        var endpoint = $"https://{appConfigurationName}.azconfig.io";

                        options.Connect(new Uri(endpoint), new DefaultAzureCredential())
                            .ConfigureKeyVault(kv =>
                            {
                                kv.SetCredential(new DefaultAzureCredential());
                            })
                            .ConfigureRefresh(opt =>
                            {
                                opt.Register("Sentinel", true);
                                opt.SetCacheExpiration(TimeSpan.FromMinutes(5));
                            })
                            .Select(KeyFilter.Any)
                            .Select(KeyFilter.Any, settings["Azure:AppConfigLabelFilter"]);
                    });
                }
            });

        builder.ConfigureLogging((context, logging) =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddApplicationInsightsWebJobs(c 
                => c.ConnectionString = context.Configuration["ApplicationInsights:ConnectionString"]);
        });

        builder.UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureServices((hostContext, services) =>
            {
                var rep = new BlobRepository(hostContext.Configuration["BlobStorage:ConnectionString"], hostContext.Configuration["BlobStorage:ContainerName"]);

                var walletPath = hostContext.Configuration["WalletFileDir"];
                Directory.CreateDirectory(walletPath);

                rep.Download(hostContext.Configuration["BlobStorage:WalletFileName"], walletPath + "\\cwallet.sso");
                Console.WriteLine("Created wallet file at: " + walletPath);

                services.AddApplicationInsightsTelemetryWorkerService(o=> 
                    o.ConnectionString = hostContext.Configuration["ApplicationInsights:ConnectionString"]); 

                var connectionString = hostContext.Configuration["EnvironmentName"] == "Development" 
                    ? hostContext.Configuration["ProcosysDb"]
                    : hostContext.Configuration["ConnectionString"];
                
                services.AddDbContext(connectionString);
                services.AddTopicClients(
                    hostContext.Configuration["ServiceBusConnectionString"],
                    hostContext.Configuration["TopicNames"]);
                services.AddRepositories();
                services.AddServices();

                services.AddHostedService<TimedWorkerService>();
            });

        return builder;
    }
}
