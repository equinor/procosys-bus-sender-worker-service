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

        public List<KeyValuePair<PcsTopic, string>> Subscriptions { get; } = new List<KeyValuePair<PcsTopic, string>>();
        /**
         * Renew lease interval for service bus subscriptions in milliseconds
         */
        public int RenewLeaseInterval { get; private set; }

        public PcsServiceBusConfig WithRenewLeaseInterval(int renewLeaseInterval)
        {
            RenewLeaseInterval = renewLeaseInterval;
            return this;
        }

        public PcsServiceBusConfig WithSubscription(PcsTopic pcsTopic, string subscriptionName)
        {
            Subscriptions.Add(new KeyValuePair<PcsTopic, string>(pcsTopic, subscriptionName));
            return this;
        }

        public Uri LeaderElectorUrl { get; private set; }

        public PcsServiceBusConfig WithLeaderElector(string leaderElectorUri)
        {
            LeaderElectorUrl = new Uri(leaderElectorUri);
            return this;
        }
    }
}
