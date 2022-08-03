using Equinor.ProCoSys.PcsServiceBus.Receiver;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;


namespace Equinor.ProCoSys.PcsServiceBusTests;

[TestClass]
public class PcsBusReceiverTests
{
    private PcsBusReceiver _dut;
    private Mock<IPcsServiceBusProcessors> _processors;
    ServiceBusProcessorOptions _options;
    private Mock<ILogger<PcsBusReceiver>> _logger;
    private Mock<IBusReceiverService> _busReceiverService;
    private Mock<ILeaderElectorService> _leaderElectorService;

    [TestInitialize]
    public void Setup()
    {
        _logger = new Mock<ILogger<PcsBusReceiver>>();

        _processors = new Mock<IPcsServiceBusProcessors>();
        _processors.Setup(c => c.RenewLeaseInterval).Returns(10000);
        _processors.Setup(c
            => c.RegisterPcsMessageHandler(
                It.IsAny<Func<IPcsServiceBusProcessor, ProcessMessageEventArgs, Task>>())
        );

        _busReceiverService = new Mock<IBusReceiverService>();
        _leaderElectorService = new Mock<ILeaderElectorService>();
        _leaderElectorService.Setup(l => l.CanProceedAsLeader(It.IsAny<Guid>())).Returns(Task.FromResult(true));

        _dut = new PcsBusReceiver(_logger.Object, _processors.Object, new SingletonBusReceiverServiceFactory(_busReceiverService.Object), _leaderElectorService.Object);
    }

    [TestMethod]
    public void StopAsync_ShouldCallCloseAllAsyncOnce()
    {
        _dut.StopAsync(new CancellationToken());

        _processors.Verify(c => c.CloseAllAsync(), Times.Once);
    }

    [TestMethod]
    public async void StartAsync_ShouldCallStartProcessAsyncOnce()
    {
        await _dut.StartAsync(new CancellationToken());

        _processors.Verify(c => c.StartProcessingAsync(), Times.Once);
    }

    [TestMethod]
    public void StartAsync_ShouldVerifyRegisterPcsMessageHandlerWasNotCalledOnStartAsync()
    {
        _dut.StartAsync(new CancellationToken());

        _processors.Verify(c => c.RegisterPcsMessageHandler(It.IsAny<Func<IPcsServiceBusProcessor, ProcessMessageEventArgs,  Task>>()), Times.Never);
        Assert.IsNull(_options);
    }

    [TestMethod]
    public void StartAsync_ShouldVerifyRegisterPcsMessageHandlerWasCalledAfterTimerStartOnStartAsync()
    {
        _dut.StartAsync(new CancellationToken());

        Thread.Sleep(7000);
        _processors.Verify(c => c.RegisterPcsMessageHandler(It.IsAny<Func<IPcsServiceBusProcessor, ProcessMessageEventArgs, Task>>()), Times.Once);
       // Assert.IsNotNull(_options);
    }

    [TestMethod]
    public void StartAsync_ShouldVerifyRegisterPcsMessageHandlerWasNotCalledAfterTimerStartWhenNotProceedAsLeaderOnStartAsync()
    {
        _leaderElectorService.Setup(l => l.CanProceedAsLeader(It.IsAny<Guid>())).Returns(Task.FromResult(false));
        _dut.StartAsync(new CancellationToken());

        Thread.Sleep(7000);
        _processors.Verify(c => c.RegisterPcsMessageHandler(It.IsAny<Func<IPcsServiceBusProcessor, ProcessMessageEventArgs,Task>>()), Times.Never);
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

        _processors.Verify(c => c.RegisterPcsMessageHandler(It.IsAny<Func<IPcsServiceBusProcessor, ProcessMessageEventArgs, Task>>()), Times.Once);
       // Assert.AreEqual(1, _options.MaxConcurrentCalls);
    }

    [TestMethod]
    public async Task ProcessMessageAsync_ShouldCallProcessMessageAsync()
    {
        var pcsProcessor = new Mock<IPcsServiceBusProcessor>();
        var processor = new Mock<ServiceBusReceiver>();
        
        // mock
        var rootOperationId = "operation-id-123";
        var parentId = "parent-id-123";
        var applicationProperties = new Dictionary<string, object>();
         applicationProperties.Add("OperationId", rootOperationId);
         applicationProperties.Add("ParentId", parentId);
        var serviceBusReceivedMessage = ServiceBusModelFactory.ServiceBusReceivedMessage(
            body: BinaryData.FromString($"{{\"Plant\" : \"asdf\", \"ProjectName\" : \"ew2f\", \"Description\" : \"sdf\"}}"),
            properties: applicationProperties,
            deliveryCount: 1);
        var messageEventArgs = new ProcessMessageEventArgs(serviceBusReceivedMessage, processor.Object, new CancellationToken());

        var lockToken = Guid.NewGuid();

        SetLockToken(messageEventArgs, lockToken);
        await _dut.ProcessMessagesAsync(pcsProcessor.Object, messageEventArgs);
        
        _busReceiverService.Verify(b => b.ProcessMessageAsync(pcsProcessor.Object.PcsTopic, Encoding.UTF8.GetString(messageEventArgs.Message.Body), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static void SetLockToken(ProcessMessageEventArgs message, Guid lockToken)
    {
        var systemProperties = message.CancellationToken;
        var type = systemProperties.GetType();
        type.GetMethod("set_LockTokenGuid", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.Invoke(systemProperties, new object[] { lockToken });
        type.GetMethod("set_SequenceNumber", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.Invoke(systemProperties, new object[] { 0 });
    }


}