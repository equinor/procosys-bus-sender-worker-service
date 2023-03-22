using System;
using System.Net.Http;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Sender;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.PcsServiceBus;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddPcsServiceBusIntegration(this IServiceCollection services, Action<PcsServiceBusConfig> options)
    {
        var optionsBuilder = new PcsServiceBusConfig();
        options(optionsBuilder);
        var pcsSubscriptionProcessors = CreateSubscriptionProcessors(optionsBuilder);
        services.AddSingleton<IPcsServiceBusProcessors>(pcsSubscriptionProcessors);

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

    private static PcsServiceBusProcessors CreateSubscriptionProcessors(PcsServiceBusConfig options)
    {
        var pcsProcessors = new PcsServiceBusProcessors(options.RenewLeaseIntervalMilliseconds);
        var client = new ServiceBusClient(options.ConnectionString);
        var processorOptions = new ServiceBusProcessorOptions
        {
            MaxConcurrentCalls = 1,
            AutoCompleteMessages = false,
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        };

        options.Subscriptions.ForEach(
            topicInfo =>
            {
                var topicInfoTopicPath = string.IsNullOrWhiteSpace(topicInfo.topicPath)
                    ? topicInfo.pcsTopic.ToString()
                    : topicInfo.topicPath;
                var subscriptionName = options.ReadFromDeadLetterQueue
                    ? $"{topicInfo.subscrition}/$deadletterqueue"
                    : topicInfo.subscrition;

                pcsProcessors.Add(
                    new PcsServiceBusProcessor(client, topicInfoTopicPath, subscriptionName, processorOptions,
                        topicInfo.pcsTopic));
            });
        return pcsProcessors;
    }

    public static void AddTopicClients(this IServiceCollection services, string serviceBusConnectionString, string topicNames)
    {
        var topics = topicNames.Split(',');
        var pcsBusSender = new PcsBusSender();
        var options = new ServiceBusClientOptions { EnableCrossEntityTransactions = true };
        var client = new ServiceBusClient(serviceBusConnectionString, options);
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
