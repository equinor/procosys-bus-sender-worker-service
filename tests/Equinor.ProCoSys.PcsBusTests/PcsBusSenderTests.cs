using Equinor.ProCoSys.PcsServiceBus.Sender;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Equinor.ProCoSys.PcsServiceBusTests;

[TestClass]
public class PcsBusSenderTests
{
    private PcsBusSender _dut;
    private Mock<ServiceBusSender> _topicClient1, _topicClient2;
    private const string TopicName1 = "Topic1";
    private const string TopicName2 = "Topic2";

    [TestInitialize]
    public void Setup()
    {
        _topicClient1 = new Mock<ServiceBusSender>();
        _topicClient2 = new Mock<ServiceBusSender>();
        _dut = new PcsBusSender();

        _dut.AddServiceBusSender(TopicName1, _topicClient1.Object);
        _dut.AddServiceBusSender(TopicName2, _topicClient2.Object);
    }

    [TestMethod]
    [ExpectedException(typeof(Exception))]
    public async Task SendAsync_ShouldThrowExceptionIfTopicNotRegistered()
    {
        // Arrange
        var message = $@"{{One small {Guid.NewGuid()}}}";

        // Act
        await _dut.SendAsync("AnUnknownTopic", message);
    }

    [TestMethod]
    public async Task SendAsync_ShouldOnlySendViaCorrectTopicClient()
    {
        // Arrange
        var message = $@"{{One small {Guid.NewGuid()}}}";

        // Act
        await _dut.SendAsync(TopicName1, message);

        // Assert
        _topicClient1.Verify(t => t.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        _topicClient2.Verify(t => t.SendMessageAsync(It.IsAny<ServiceBusMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [TestMethod]
    public async Task CloseAll_ShouldCloseAllTopicClients()
    {
        // Act
        await _dut.CloseAllAsync();

        // Assert
        _topicClient1.Verify(t => t.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
        _topicClient2.Verify(t => t.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}