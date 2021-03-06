﻿using System;
using Equinor.ProCoSys.PcsServiceBus.Receiver;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Sender;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.PcsServiceBus
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddPcsServiceBusIntegration(this IServiceCollection services, Action<PcsServiceBusConfig> options)
        {
            var optionsBuilder = new PcsServiceBusConfig();
            options(optionsBuilder);

            var pcsSubscriptionClients = new PcsSubscriptionClients();
            optionsBuilder.Subscriptions.ForEach(
                s => 
                    pcsSubscriptionClients.Add(
                        new PcsSubscriptionClient(optionsBuilder.ConnectionString, s.Key, s.Value)));
            services.AddSingleton<IPcsSubscriptionClients>(pcsSubscriptionClients);

            services.AddHostedService<PcsBusReceiver>();

            return services;
        }

        public static void AddTopicClients(this IServiceCollection services, string serviceBusConnectionString, string topicNames)
        {
            var topics = topicNames.Split(',');
            var pcsBusSender = new PcsBusSender();
            foreach (var topicName in topics)
            {
                if (!string.IsNullOrWhiteSpace(topicName))
                {
                    var topicClient = new TopicClient(serviceBusConnectionString, topicName);
                    pcsBusSender.Add(topicName, topicClient);
                }
            }

            services.AddSingleton<IPcsBusSender>(pcsBusSender);
        }
    }
}
