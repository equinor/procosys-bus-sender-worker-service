using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Microsoft.Azure.ServiceBus;
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
            _timer = new Timer(CanProceedAsLeaderCheck, null, 5000, Timeout.Infinite);

            return Task.CompletedTask;
        }

        private async void CanProceedAsLeaderCheck(object state)
        {
            try
            {
                _logger.LogDebug($"CanProceedAsLeaderCheck started do work at: {DateTimeOffset.Now}");

                if (IsLeader)
                {
                    await RenewLease();
                }
                else
                {
                    await TryBecomeLeader();
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"CanProceedAsLeaderCheck failed at: {DateTimeOffset.Now}");
            }
            finally
            {
                _timer.Change(_subscriptionClients.RenewLeaseInterval, Timeout.Infinite);
            }
        }

        private async Task TryBecomeLeader()
        {
            var canProceedAsLeader = await _leaderElectorService.CanProceedAsLeader(_receiverId);

            if (canProceedAsLeader)
            {
                _logger.LogInformation($"CanProceedAsLeaderCheck, lease obtained at: {DateTimeOffset.Now}");
                IsLeader = true;
                StartMessageReceiving();
            }
        }

        private async Task RenewLease()
        {
            var canProceedAsLeader = await _leaderElectorService.CanProceedAsLeader(_receiverId);

            if (!canProceedAsLeader)
            {
                // Suddenly lost role as leader for some strange reason. Leader elector could have restarted
                _logger.LogWarning( $"CanProceedAsLeaderCheck, lease lost at: {DateTimeOffset.Now}");
                IsLeader = false;
                StopMessageReceiving();
            }
            else
            {
                _logger.LogDebug($"CanProceedAsLeaderCheck, lease renewed at: {DateTimeOffset.Now}");
            }
        }

        private bool IsLeader { get; set; }

        private void StartMessageReceiving()
        {
            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            _subscriptionClients.RegisterPcsMessageHandler(ProcessMessagesAsync, messageHandlerOptions);
        }

        private void StopMessageReceiving() => _subscriptionClients.UnregisterPcsMessageHandler();

        public async Task ProcessMessagesAsync(IPcsSubscriptionClient subscriptionClient, Message message, CancellationToken token)
        {
            try
            {
                var messageJson = Encoding.UTF8.GetString(message.Body);

                var busReceiverService = _busReceiverServiceFactory.GetServiceInstance();

                await busReceiverService.ProcessMessageAsync(subscriptionClient.PcsTopic, messageJson, token);

                await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
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
