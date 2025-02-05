using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Validation;
using Equinor.ProCoSys.PcsServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.BusSender.Worker;

// ReSharper disable once ClassNeverInstantiated.Global
public class Program
{
    public Program(IConfiguration configuration)
        => Configuration = configuration;

    private static IHostBuilder CreateHostBuilder(string[] args)
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
                            .Select(KeyFilter.Any, null)
                            .Select(KeyFilter.Any, context.HostingEnvironment.EnvironmentName)
                            .ConfigureRefresh(refreshOptions =>
                            {
                                refreshOptions.Register("Sentinel", true);
                                refreshOptions.SetCacheExpiration(TimeSpan.FromMinutes(5));
                            });
                    });
                }
                else if (settings["IsLocal"] == "True" && azConfig)
                {
                    config.AddAzureAppConfiguration(options =>
                    {
                        var tenantId = settings["AZURE_TENANT_ID"];
                        var clientId = settings["AZURE_CLIENT_ID"];
                        var clientSecret = settings["AZURE_CLIENT_SECRET"];

                        var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

                        var connectionString = settings["ConnectionStrings:AppConfig"];
                        options.Connect(connectionString)
                            .ConfigureKeyVault(kv =>
                            {
                                kv.SetCredential(credential);
                            })
                            .ConfigureRefresh(opt =>
                            {
                                opt.Register("Sentinel", true);
                                opt.SetCacheExpiration(TimeSpan.FromMinutes(5));
                            })
                            .Select(KeyFilter.Any, null)
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
                            .Select(KeyFilter.Any, null)
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
                if (hostContext.Configuration["IsLocal"] == "True")
                {
                    var localConnectionString = hostContext.Configuration["ProcosysDb"]!;
                    services.AddDbContext(localConnectionString);
                }
                else
                {
                    var rep = new BlobRepository(hostContext.Configuration["BlobStorage:ConnectionString"]!,
                        hostContext.Configuration["BlobStorage:ContainerName"]!);
                    var walletPath = hostContext.Configuration["WalletFileDir"]!;
                    Directory.CreateDirectory(walletPath);
                    rep.Download(hostContext.Configuration["BlobStorage:WalletFileName"]!);
                    var connectionString = hostContext.Configuration["ConnectionString"]!;
                    services.AddDbContext(connectionString);
                    services.AddApplicationInsightsTelemetryWorkerService(o =>
                        o.ConnectionString = hostContext.Configuration["ApplicationInsights:ConnectionString"]);
                }

                services.AddTopicClients(
                    hostContext.Configuration["ServiceBusConnectionString"]!,
                    hostContext.Configuration["TopicNames"]!);

                services.Configure<InstanceOptions>(hostContext.Configuration.GetSection("InstanceOptions"));
                services.AddServices();
                services.AddRepositories();
                services.AddHostedService<TimedWorkerService>();
                services.AddMemoryCache();
            });
        return builder;
    }

    public static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        var serviceProvider = host.Services;
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var instanceOptions = serviceProvider.GetRequiredService<IOptions<InstanceOptions>>();
        var plantsByInstances = configuration.GetRequiredSection("PlantsByInstance").Get<List<PlantsByInstance>>();

        ConfigurationValidator.ValidatePlantsByInstance(plantsByInstances);
        ConfigurationValidator.ValidateInstanceOptions(instanceOptions.Value);
        await host.RunAsync();
    }

    public IConfiguration Configuration { get; }
}
