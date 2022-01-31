using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Equinor.ProCoSys.PcsServiceBus.Sender;
using Equinor.ProCoSys.PcsServiceBus.Sender.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Equinor.ProCoSys.PcsServiceBus
{
    public static class IServiceCollectionExtensions
    {
        public static async Task<IServiceCollection> AddPcsServiceBusIntegrationAsync(this IServiceCollection services, Action<PcsServiceBusConfig> options)
        {
            var optionsBuilder = new PcsServiceBusConfig();
            options(optionsBuilder);
            await using var client = new ServiceBusClient(optionsBuilder.ConnectionString);


            var pcsSubscriptionClients = new PcsSubscriptionClients(optionsBuilder.RenewLeaseIntervalMilliSec);
            optionsBuilder. Subscriptions.ForEach(
                s => 
                    pcsSubscriptionClients.Add(
                        new PcsSubscriptionClient(client, s.Key, s.Value)));
            services.AddSingleton<IPcsSubscriptionClients>(pcsSubscriptionClients);

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
            await using var client = new ServiceBusClient(serviceBusConnectionString);
            foreach (var topicName in topics)
            {
                if (!string.IsNullOrWhiteSpace(topicName))
                {
                    var sender = client.CreateSender(topicName);
                    pcsBusSender.Add(topicName, sender);
                }
            }

            services.AddSingleton<IPcsBusSender>(pcsBusSender);
        }
    }
}
