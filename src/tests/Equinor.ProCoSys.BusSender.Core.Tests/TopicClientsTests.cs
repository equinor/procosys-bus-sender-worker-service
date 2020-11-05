using System;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Equinor.ProCoSys.BusSender.Core.Tests
{
    [TestClass]
    public class TopicClientsTests
    {
        private TopicClients _dut;
        private Mock<ITopicClient> _topicClientMock1;
        private Mock<ITopicClient> _topicClientMock2;

        [TestInitialize]
        public void Setup()
        {
            _dut = new TopicClients();
            _topicClientMock1 = new Mock<ITopicClient>();
            _topicClientMock2 = new Mock<ITopicClient>();
            _dut.Add("topic1", _topicClientMock1.Object);
            _dut.Add("topic2", _topicClientMock2.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public async Task ShouldCrashOnNoTopicClients()
        {
            var emptyTopicClients = new TopicClients();
            await emptyTopicClients.Send("test", "test");
        }

        [TestMethod]
        public async Task CloseAllShouldCallAllTopicClients()
        {
            await _dut.CloseAllAsync();
            _topicClientMock1.Verify(t => t.CloseAsync(),Times.Once);
            _topicClientMock2.Verify(t => t.CloseAsync(), Times.Once);
        }

        [TestMethod]
        public async Task OnlySendOnCorrectTopicClient()
        {
            await _dut.Send("topic1", "abc");
            _topicClientMock1.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Once);
            _topicClientMock2.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Never);
        }

    }
}
