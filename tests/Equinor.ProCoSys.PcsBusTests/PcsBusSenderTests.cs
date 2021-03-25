using System;
using System.Text;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Sender;
using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.PcsServiceBusTests
{
    [TestClass]
    public class PcsBusSenderTests
    {
        private PcsBusSender _dut;
        private Mock<ITopicClient> _topicClient1, _topicClient2;
        private const string TopicName1 = "Topic1";
        private const string TopicName2 = "Topic2";

        [TestInitialize]
        public void Setup()
        {
            _topicClient1 =  new Mock<ITopicClient>();
            _topicClient2 = new Mock<ITopicClient>();
            _dut = new PcsBusSender();
            
            _dut.Add(TopicName1, _topicClient1.Object);
            _dut.Add(TopicName2, _topicClient2.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task SendAsync_ShouldThrowExceptionIfTopicNotRegistered()
        {
            // Arrange
            var message = new Message(Encoding.UTF8.GetBytes($@"{{One small {Guid.NewGuid()}}}"));

            // Act
            await _dut.SendAsync("AnUnknownTopic", message);
        }

        [TestMethod]
        public async Task SendAsync_ShouldOnlySendViaCorrectTopicClient()
        {
            // Arrange
            var message = new Message(Encoding.UTF8.GetBytes($@"{{One small {Guid.NewGuid()}}}"));

            // Act
            await _dut.SendAsync(TopicName1, message);

            // Assert
            _topicClient1.Verify(t => t.SendAsync(message), Times.Once);
            _topicClient2.Verify(t => t.SendAsync(message), Times.Never);
        }

        [TestMethod]
        public async Task CloseAll_ShouldCloseAllTopicClients()
        {
            // Act
            await _dut.CloseAllAsync();

            // Assert
            _topicClient1.Verify(t => t.CloseAsync(), Times.Once);
            _topicClient2.Verify(t => t.CloseAsync(), Times.Once);
        }
    }
}
