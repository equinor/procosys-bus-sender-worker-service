using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Receiver;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.PcsServiceBusTests
{
    [TestClass]
    public class PcsBusReceiverTests
    {
        private PcsBusReceiver _dut;
        private Mock<IPcsSubscriptionClients> _clients;
        MessageHandlerOptions _options;
        private Mock<ILogger<PcsBusReceiver>> _logger;
        private Mock<IBusReceiverService> _busReceiverService;
        private Mock<ILeaderElectorService> _leaderElectorService;

        [TestInitialize]
        public void Setup()
        {
            _logger = new Mock<ILogger<PcsBusReceiver>>();

            _clients = new Mock<IPcsSubscriptionClients>();
            _clients.Setup(c => c.RenewLeaseInterval).Returns(10000);
            _clients.Setup(c
                => c.RegisterPcsMessageHandler(
                    It.IsAny<Func<IPcsSubscriptionClient, Message, CancellationToken, Task>>(),
                    It.IsAny<MessageHandlerOptions>())
            ).Callback<Func<IPcsSubscriptionClient, Message, CancellationToken, Task> , MessageHandlerOptions>((_,options) => _options = options);

            _busReceiverService = new Mock<IBusReceiverService>();
            _leaderElectorService = new Mock<ILeaderElectorService>();
            _leaderElectorService.Setup(l => l.CanProceedAsLeader(It.IsAny<Guid>())).Returns(Task.FromResult(true));

            _dut = new PcsBusReceiver(_logger.Object, _clients.Object, new SingletonBusReceiverServiceFactory(_busReceiverService.Object), _leaderElectorService.Object);
        }

        [TestMethod]
        public void StopAsync_ShouldCallCloseAllAsyncOnce()
        {
            _dut.StopAsync(new CancellationToken());

            _clients.Verify(c => c.CloseAllAsync(), Times.Once );
        }

        [TestMethod]
        public void StartAsync_ShouldVerifyRegisterPcsMessageHandlerWasNotCalledOnStartAsync()
        {
            _dut.StartAsync(new CancellationToken());

            _clients.Verify(c => c.RegisterPcsMessageHandler(It.IsAny<Func<IPcsSubscriptionClient, Message, CancellationToken, Task>>(), It.IsAny<MessageHandlerOptions>()), Times.Never);
            Assert.IsNull(_options);
        }

        [TestMethod]
        public void StartAsync_ShouldVerifyRegisterPcsMessageHandlerWasCalledAfterTimerStartOnStartAsync()
        {
            _dut.StartAsync(new CancellationToken());

            Thread.Sleep(7000);
            _clients.Verify(c => c.RegisterPcsMessageHandler(It.IsAny<Func<IPcsSubscriptionClient, Message, CancellationToken, Task>>(), It.IsAny<MessageHandlerOptions>()), Times.Once);
            Assert.IsNotNull(_options);
        }

        [TestMethod]
        public void StartAsync_ShouldVerifyRegisterPcsMessageHandlerWasNotCalledAfterTimerStartWhenNotProceedAsLeaderOnStartAsync()
        {
            _leaderElectorService.Setup(l => l.CanProceedAsLeader(It.IsAny<Guid>())).Returns(Task.FromResult(false));
            _dut.StartAsync(new CancellationToken());

            Thread.Sleep(7000);
            _clients.Verify(c => c.RegisterPcsMessageHandler(It.IsAny<Func<IPcsSubscriptionClient, Message, CancellationToken, Task>>(), It.IsAny<MessageHandlerOptions>()), Times.Never);
            Assert.IsNull(_options);
        }

        [TestMethod]
        public void StartAsync_ShouldCallCanProceedAsLeader()
        {
            _dut.StartAsync(new CancellationToken());
            Thread.Sleep(7000);
            _leaderElectorService.Verify(l => l.CanProceedAsLeader(It.IsAny<Guid>()), Times.Once);
        }

        [TestMethod]
        public void StartAsync_ShouldNotCallCanProceedAsLeaderImmediately()
        {
            _dut.StartAsync(new CancellationToken());
            _leaderElectorService.Verify(l => l.CanProceedAsLeader(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public async Task StartAsync_ShouldVerifyRegisterPcsMessageHandlerWasCalledAndMaxConcurrentCallsWasSet2()
        {
            await _dut.StartAsync(new CancellationToken());

            Thread.Sleep(7000);

            _clients.Verify(c => c.RegisterPcsMessageHandler(It.IsAny<Func<IPcsSubscriptionClient, Message, CancellationToken, Task>>(), It.IsAny<MessageHandlerOptions>()), Times.Once);
            Assert.AreEqual(1, _options.MaxConcurrentCalls);
        }

        [TestMethod]
        public async Task ProcessMessageAsync_ShouldCallProcessMessageAsync()
        {
            var client = new Mock<IPcsSubscriptionClient>();
            var message = new Message(Encoding.UTF8.GetBytes($"{{\"Plant\" : \"asdf\", \"ProjectName\" : \"ew2f\", \"Description\" : \"sdf\"}}"));

            var lockToken = Guid.NewGuid();

            SetLockToken(message, lockToken);
            await _dut.ProcessMessagesAsync(client.Object, message, new CancellationToken());

            _busReceiverService.Verify(b => b.ProcessMessageAsync(client.Object.PcsTopic, Encoding.UTF8.GetString(message.Body), It.IsAny<CancellationToken>()), Times.Once);
            client.Verify(c => c.CompleteAsync(lockToken.ToString()), Times.Once);
        }

        private static void SetLockToken(Message message, Guid lockToken)
        {
            var systemProperties = message.SystemProperties;
            var type = systemProperties.GetType();
            type.GetMethod("set_LockTokenGuid", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(systemProperties, new object[] {lockToken});
            type.GetMethod("set_SequenceNumber", BindingFlags.Instance | BindingFlags.NonPublic)
                ?.Invoke(systemProperties, new object[] {0});
        }
    }
}
