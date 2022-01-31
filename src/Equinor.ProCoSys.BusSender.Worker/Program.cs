﻿using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Equinor.ProCoSys.BusSender.Infrastructure;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.BusSender.Worker
{
    public class Program
    {
        public IConfiguration Configuration { get; }


        public Program(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static async Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            ILogger? logger = host.Services.GetService<ILogger<Program>>();
            await host.RunAsync();
        }

        //public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((context, config) =>
                {
                    config.AddUserSecrets<Program>();
                    var settings = config.Build();

                    if (settings["EnvironmentName"] == "Local")
                    {
                        config.AddAzureAppConfiguration(options =>
                        {

                            options.Connect(settings["AppConfig"])
                                .ConfigureKeyVault(kv =>
                                {
                                    kv.SetCredential(new DefaultAzureCredential());
                                })
                                .ConfigureRefresh(options =>
                                {
                                    options.Register("Sentinel", true);
                                    options.SetCacheExpiration(TimeSpan.FromMinutes(5));
                                })
                                .Select(KeyFilter.Any, LabelFilter.Null)
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
                                .ConfigureRefresh(options =>
                                {
                                    options.Register("Sentinel", true);
                                    options.SetCacheExpiration(TimeSpan.FromMinutes(5));
                                })
                                .Select(KeyFilter.Any, LabelFilter.Null)
                                .Select(KeyFilter.Any, settings["Azure:AppConfigLabelFilter"]);
                        });
                    }
                });

            builder.ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddConsole();
                logging.AddApplicationInsightsWebJobs(c => c.InstrumentationKey = context.Configuration["ApplicationInsights:InstrumentationKey"]);
            });

            builder.UseContentRoot(Directory.GetCurrentDirectory())
           .ConfigureServices((hostContext, services) =>
           {
               var rep = new BlobRepository(hostContext.Configuration["BlobStorage:ConnectionString"], hostContext.Configuration["BlobStorage:ContainerName"]);

               string walletPath = hostContext.Configuration["WalletFileDir"];
               Directory.CreateDirectory(walletPath);

               rep.Download(hostContext.Configuration["BlobStorage:WalletFileName"], walletPath + "\\cwallet.sso");
               Console.WriteLine("Created wallet file at: " + walletPath);

               services.AddApplicationInsightsTelemetryWorkerService(hostContext.Configuration["ApplicationInsights:InstrumentationKey"]);

               services.AddDbContext(hostContext.Configuration["ConnectionString"]);
               services.AddTopicClients(
                   hostContext.Configuration["ServiceBusConnectionString"],
                   hostContext.Configuration["TopicNames"]);
               services.AddRepositories();
               services.AddServices();

               services.AddHostedService<TimedWorkerService>();
           });

            return builder;
        }



        //Host.CreateDefaultBuilder(args)
        //    .UseWindowsService()
        //    .ConfigureAppConfiguration((_, config) =>
        //    {
        //        config = new ConfigurationBuilder()
        //            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        //            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        //    })
        //    .UseContentRoot(Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName))
        //    .ConfigureLogging(builder =>
        //    {
        //        builder.AddApplicationInsights();
        //        builder.AddFilter<Microsoft.Extensions.Logging.ApplicationInsights.ApplicationInsightsLoggerProvider>
        //                     ("", LogLevel.Information);
        //    })
        //    .ConfigureServices((hostContext, services) =>
        //    {
        //        services.AddApplicationInsightsTelemetryWorkerService();
        //        services.AddDbContext(hostContext.Configuration["ConnectionString"]);
        //        services.AddTopicClients(
        //            hostContext.Configuration["ServiceBusConnectionString"],
        //            hostContext.Configuration["TopicNames"]);
        //        services.AddRepositories();
        //        services.AddServices();

        //        services.AddHostedService<TimedWorkerService>();
        //    });
    }
}
