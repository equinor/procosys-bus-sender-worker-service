using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Core.Interfaces;
using Equinor.ProCoSys.BusSender.Core.Models;
using Equinor.ProCoSys.BusSender.Core.Services;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Range = Moq.Range;

namespace Equinor.ProCoSys.BusSender.Core.Tests
{
    [TestClass]
    public class BusSenderServiceTests
    {
        private BusSenderService _dut;
        private Mock<IUnitOfWork> _iUnitOfWork;
        private Mock<ITopicClient> _topicClientMock1, _topicClientMock2, _topicClientMock3;

        [TestInitialize]
        public void Setup()
        {
            var topicClients = new TopicClients();
            _topicClientMock1 = new Mock<ITopicClient>();
            _topicClientMock2 = new Mock<ITopicClient>();
            _topicClientMock3 = new Mock<ITopicClient>();
            topicClients.Add("topic1", _topicClientMock1.Object);
            topicClients.Add("topic2", _topicClientMock2.Object);
            topicClients.Add("topic3", _topicClientMock3.Object);

            var busEvents = new List<BusEvent>
            {
                new BusEvent
                {
                    Created = DateTime.Now.AddMinutes(-10),
                    Event = "topic2",
                    Sent = 0,
                    Id = 1,
                    Message = "Message 10 minutes ago not sent"
                },
                new BusEvent
                {
                    Created = DateTime.Now.AddMinutes(-10),
                    Event = "topic3",
                    Sent = 0,
                    Id = 1,
                    Message = "Message 10 minutes ago not sent"
                }
            };
            var busEventRepository = new Mock<IBusEventRepository>();
            _iUnitOfWork = new Mock<IUnitOfWork>();

            busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk()).Returns(() => Task.FromResult(busEvents));
            _dut = new BusSenderService(topicClients, busEventRepository.Object, _iUnitOfWork.Object, new Mock<ILogger<BusSenderService>>().Object);
        }

        [TestMethod]
        public async Task CloseOnAllTopicClients()
        {
            await _dut.StopService();
            _topicClientMock1.Verify(t => t.CloseAsync(), Times.Once);
            _topicClientMock2.Verify(t => t.CloseAsync(), Times.Once);
            _topicClientMock3.Verify(t => t.CloseAsync(), Times.Once);
        }

        [TestMethod]
        public async Task SendMessagesViaCorrectTopicClients()
        {
            await _dut.SendMessageChunk();
            _topicClientMock1.Verify(t => t.SendAsync(It.IsAny<Message>()),Times.Never);
            _topicClientMock2.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Once);
            _topicClientMock3.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Once);
        }

        [TestMethod]
        public async Task SendMessageSavesAfterEachSend()
        {
            await _dut.SendMessageChunk();
            _iUnitOfWork.Verify(t => t.SaveChangesAsync(), Times.Between(2,2,Range.Inclusive));
        }
    }
}
