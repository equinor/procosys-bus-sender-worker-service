using System;
using System.Collections.Generic;

namespace Equinor.ProCoSys.PcsServiceBus;

public class PcsServiceBusConfig
{
    public PcsServiceBusConfig UseBusConnection(string connectionString)
    {
        ConnectionString = connectionString;
        return this;
    }

    public PcsServiceBusConfig WithLeaderElector(string leaderElectorUri)
    {
        LeaderElectorUrl = new Uri(leaderElectorUri);
        return this;
    }

    /// <summary>
    ///     If true, topic messages will be fetched from Dead Letter Queue instead of normal. This is for special cases, use
    ///     with caution!
    /// </summary>
    public PcsServiceBusConfig WithReadFromDeadLetterQueue(bool readFromDeadLetterQueue)
    {
        ReadFromDeadLetterQueue = readFromDeadLetterQueue;
        return this;
    }

    public PcsServiceBusConfig WithRenewLeaseInterval(int renewLeaseIntervalMilliseconds)
    {
        RenewLeaseIntervalMilliseconds = renewLeaseIntervalMilliseconds;
        return this;
    }

    public PcsServiceBusConfig WithSubscription(string pcsTopic, string subscriptionName)
    {
        Subscriptions.Add(new ValueTuple<string, string?, string>(pcsTopic, null, subscriptionName));
        return this;
    }

    public PcsServiceBusConfig WithSubscription(string pcsTopic, string topicPath, string subscriptionName)
    {
        Subscriptions.Add(new ValueTuple<string, string?, string>(pcsTopic, topicPath, subscriptionName));
        return this;
    }

    public string? ConnectionString { get; set; }

    public bool ReadFromDeadLetterQueue { get; set; }

    public List<(string pcsTopic, string? topicPath, string subscrition)> Subscriptions { get; } = new();

    public int RenewLeaseIntervalMilliseconds { get; private set; }

    public Uri? LeaderElectorUrl { get; private set; }
}
