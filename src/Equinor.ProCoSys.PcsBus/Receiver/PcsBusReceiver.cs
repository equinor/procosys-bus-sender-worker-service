﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Equinor.ProCoSys.PcsServiceBus.Receiver
{
    public class PcsBusReceiver : IHostedService
    {
        private readonly ILogger<PcsBusReceiver> _logger;
        private readonly IPcsSubscriptionClients _subscriptionClients;
        private readonly IBusReceiverServiceFactory _busReceiverServiceFactory;
        private readonly ILeaderElectorService _leaderElectorService;
        private Timer _timer;
        private readonly Guid _receiverId = Guid.NewGuid();

        public PcsBusReceiver(
            ILogger<PcsBusReceiver> logger,
            IPcsSubscriptionClients subscriptionClients,
            IBusReceiverServiceFactory busReceiverServiceFactory,
            ILeaderElectorService leaderElectorService)
        {
            _logger = logger;
            _subscriptionClients = subscriptionClients;
            _busReceiverServiceFactory = busReceiverServiceFactory;
            _leaderElectorService = leaderElectorService;

            if (_subscriptionClients.RenewLeaseInterval == 0)
            {
                throw new Exception("Lease interval must be a positive integer");
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CanProceedAsLeaderCheckAsync, null, 5000, Timeout.Infinite);

            return Task.CompletedTask;
        }

        private async void CanProceedAsLeaderCheckAsync(object state)
        {
            try
            {
                _logger.LogDebug($"CanProceedAsLeaderCheckAsync started do work at: {DateTimeOffset.Now}");

                if (IsLeader)
                {
                    await RenewLeaseAsync();
                }
                else
                {
                    await TryBecomeLeaderAsync();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"CanProceedAsLeaderCheckAsync failed at: {DateTimeOffset.Now}");
            }
            finally
            {
                _timer.Change(_subscriptionClients.RenewLeaseInterval, Timeout.Infinite);
            }
        }

        private async Task TryBecomeLeaderAsync()
        {
            var canProceedAsLeader = await _leaderElectorService.CanProceedAsLeader(_receiverId);

            if (canProceedAsLeader)
            {
                _logger.LogInformation($"CanProceedAsLeaderCheckAsync, lease obtained at: {DateTimeOffset.Now}");
                IsLeader = true;
                StartMessageReceiving();
            }
        }

        private async Task RenewLeaseAsync()
        {
            var canProceedAsLeader = await _leaderElectorService.CanProceedAsLeader(_receiverId);

            if (!canProceedAsLeader)
            {
                // Suddenly lost role as leader for some strange reason. Leader elector could have restarted
                _logger.LogWarning( $"CanProceedAsLeaderCheckAsync, lease lost at: {DateTimeOffset.Now}");
                IsLeader = false;
                StopMessageReceiving();
            }
            else
            {
                _logger.LogDebug($"CanProceedAsLeaderCheckAsync, lease renewed at: {DateTimeOffset.Now}");
            }
        }

        private bool IsLeader { get; set; }

        private void StartMessageReceiving()
        {
            _subscriptionClients.RegisterPcsMessageHandler(ProcessMessagesAsync);
        }

        private void StopMessageReceiving() => _subscriptionClients.UnregisterPcsMessageHandler();

        public async Task ProcessMessagesAsync(IPcsSubscriptionClient subscriptionClient, ProcessMessageEventArgs args, CancellationToken token)
        {
            try
            {
                var messageJson = args.Message.Body.ToString();

                var busReceiverService = _busReceiverServiceFactory.GetServiceInstance();

                await busReceiverService.ProcessMessageAsync(subscriptionClient.PcsTopic, messageJson, token);

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _subscriptionClients.CloseAllAsync();

            return Task.CompletedTask;
        }

        // Use this handler to examine the exceptions received on the message pump.
        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            _logger.LogError($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
            _logger.LogError("Exception context for troubleshooting:");
            _logger.LogError($"- Endpoint: {context.Endpoint}");
            _logger.LogError($"- Entity Path: {context.EntityPath}");
            _logger.LogError($"- Executing Action: {context.Action}");
            return Task.CompletedTask;
        }
    }
}
