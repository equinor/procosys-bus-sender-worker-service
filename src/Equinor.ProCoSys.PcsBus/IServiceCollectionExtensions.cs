using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Sender;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.PcsServiceBus;

public static class IServiceCollectionExtensions
{
    public static async Task<IServiceCollection> AddPcsServiceBusIntegration(this IServiceCollection services, Action<PcsServiceBusConfig> options)
    {
        var optionsBuilder = new PcsServiceBusConfig();
        options(optionsBuilder);

        var pcsSubscriptionClients = new PcsServiceBusProcessors(optionsBuilder.RenewLeaseIntervalMilliSec);

        await using var client = new ServiceBusClient(optionsBuilder.ConnectionString);
        var processorOptions = new ServiceBusProcessorOptions()
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false
        };
        
        optionsBuilder.Subscriptions.ForEach(
            topicInfo =>
            {
                var topicInfoTopicPath = string.IsNullOrWhiteSpace(topicInfo.topicPath)
                    ? topicInfo.pcsTopic.ToString()
                    : topicInfo.topicPath;

                var subscriptionName = "";
                pcsSubscriptionClients.Add(
                    new PcsServiceBusProcessor(client,topicInfoTopicPath,subscriptionName,processorOptions, topicInfo.pcsTopic));
            });





        services.AddSingleton<IPcsServiceBusProcessors>(pcsSubscriptionClients);

        if (optionsBuilder.LeaderElectorUrl != null)
        {
            services.AddSingleton<ILeaderElectorService>(new LeaderElectorService(new HttpClient(), optionsBuilder.LeaderElectorUrl));
        }
        else
        {
            services.AddSingleton<ILeaderElectorService>(new AlwaysProceedLeaderElectorService());
        }

        services.AddHostedService<PcsBusReceiver>();

        return services;
    }

    public static async void AddTopicClients(this IServiceCollection services, string serviceBusConnectionString, string topicNames)
    {
        var topics = topicNames.Split(',');
        var pcsBusSender = new PcsBusSender();
        var options = new ServiceBusClientOptions { EnableCrossEntityTransactions = true };
        await using var client = new ServiceBusClient(serviceBusConnectionString, options);
        foreach (var topicName in topics)
        {
            if (!string.IsNullOrWhiteSpace(topicName))
            {
                var serviceBusSender = client.CreateSender(topicName);
                pcsBusSender.Add(topicName, serviceBusSender);
            }
        }

        services.AddSingleton<IPcsBusSender>(pcsBusSender);
    }
}
