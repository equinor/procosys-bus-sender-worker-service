using Equinor.ProCoSys.PcsServiceBus.Receiver;
using Equinor.ProCoSys.PcsServiceBus.Receiver.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Equinor.ProCoSys.PcsServiceBusTests;

[TestClass]
public class PcsSubscriptionClientsTests
{
    private PcsServiceBusProcessors _dut;
    private Mock<IPcsServiceBusProcessor> _client1;
    private Mock<IPcsServiceBusProcessor> _client2;

    [TestInitialize]
    public void Setup()
    {
        _dut = new PcsServiceBusProcessors(1000);
        _client1 = new Mock<IPcsServiceBusProcessor>();
        _client2 = new Mock<IPcsServiceBusProcessor>();

        _dut.Add(_client1.Object);
        _dut.Add(_client2.Object);
    }

    // [TestMethod]
    // public async Task CloseAllAsyncTestAsync()
    // {
    //     await _dut.CloseAllAsync();
    //     _client1.Verify(c => c.CloseAsync(), Times.Once);
    //     _client2.Verify(c => c.CloseAsync(), Times.Once);
    // }

    [TestMethod]
    public async Task AllMethodsWorkWithoutFailureOnEmpty()
    {
        var emptyClients = new PcsServiceBusProcessors(1000);
        var handler = new Mock<Func<IPcsServiceBusProcessor, ProcessMessageEventArgs, Task>>();
        var errorHandler = new Mock<Func<ProcessErrorEventArgs, Task>>();
        var options = new ServiceBusProcessorOptions();
        await emptyClients.CloseAllAsync();
        emptyClients.RegisterPcsEventHandlers(handler.Object,errorHandler.Object);
    }

    private Task Test(ProcessErrorEventArgs arg) => Task.CompletedTask;
}