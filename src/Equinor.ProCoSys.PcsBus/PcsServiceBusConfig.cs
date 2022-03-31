using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus
{
    public class PcsServiceBusConfig
    {
        public PcsServiceBusConfig UseBusConnection(string connectionString)
        {
            ConnectionString = connectionString;
            return this;
        }

        public string ConnectionString { get; set; }

        public bool UseDeadLetterQueue { get; set; }
        public PcsServiceBusConfig WithLeaderElector(string leaderElectorUri)
        {
            LeaderElectorUrl = new Uri(leaderElectorUri);
            return this;
        }

        public List<(PcsTopic pcsTopic, string topicPath, string subscrition)> Subscriptions { get; } = new();

        public int RenewLeaseIntervalMilliSec { get; private set; }

        public PcsServiceBusConfig WithRenewLeaseInterval(int renewLeaseIntervalMilliSec)
        {
            RenewLeaseIntervalMilliSec = renewLeaseIntervalMilliSec;
            return this;
        }

        public PcsServiceBusConfig WithSubscription(PcsTopic pcsTopic, string subscriptionName)
        {
            Subscriptions.Add(new ValueTuple<PcsTopic, string, string>(pcsTopic, null, subscriptionName));
            return this;
        }

        public PcsServiceBusConfig WithSubscription(PcsTopic pcsTopic,string topicPath, string subscriptionName)
        {
            Subscriptions.Add(new ValueTuple<PcsTopic, string, string>(pcsTopic,topicPath, subscriptionName));
            return this;
        }

        public Uri LeaderElectorUrl { get; private set; }

        /// <summary>
        /// If true, topic messages will be fetched from Dead Letter Queue instead of normal. This is for special cases, use with caution!
        /// </summary>
        public PcsServiceBusConfig WithUseDeadLetterQueue(bool useDeadLetter)
        {
            UseDeadLetterQueue = useDeadLetter;
            return this;
        }

    }
}
