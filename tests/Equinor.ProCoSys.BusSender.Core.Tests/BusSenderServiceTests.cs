﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSender.Core.Services;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.PcsServiceBus.Sender;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Range = Moq.Range;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests
{
    [TestClass]
    public class BusSenderServiceTests
    {
        private BusSenderService _dut;
        private Mock<IUnitOfWork> _iUnitOfWork;
        private Mock<ITopicClient> _topicClientMock1, _topicClientMock2, _topicClientMock3;
        private List<BusEvent> _busEvents;

        [TestInitialize]
        public void Setup()
        {
            var topicClients = new PcsBusSender();
            _topicClientMock1 = new Mock<ITopicClient>();
            _topicClientMock2 = new Mock<ITopicClient>();
            _topicClientMock3 = new Mock<ITopicClient>();
            topicClients.Add("topic1", _topicClientMock1.Object);
            topicClients.Add("topic2", _topicClientMock2.Object);
            topicClients.Add("topic3", _topicClientMock3.Object);

            _busEvents = new List<BusEvent>
            {
                new BusEvent
                {
                    Created = DateTime.Now.AddMinutes(-10),
                    Event = "topic2",
                    Sent = Status.UnProcessed,
                    Id = 1,
                    Message = "{\"Plant\":\"NGPCS_TEST_BROWN\",\"ProjectName\":\"Message 10 minutes ago not sent\"}"
                },
                new BusEvent
                {
                    Created = DateTime.Now.AddMinutes(-10),
                    Event = "topic3",
                    Sent = Status.UnProcessed,
                    Id = 1,
                    Message = "{\"Plant\" : \"PCS$HF_LNG\", \"ProjectName\" : \"M.O790C.IM.X.0324\", \"McPkgNo\" : \"8460-E015\", \"Description\" : \"	Installere bonding til JBer ved V8 område\"}"
                }
            };
            var busEventRepository = new Mock<IBusEventRepository>();
            _iUnitOfWork = new Mock<IUnitOfWork>();

            busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk()).Returns(() => Task.FromResult(_busEvents));
            _dut = new BusSenderService(topicClients, busEventRepository.Object, _iUnitOfWork.Object, new Mock<ILogger<BusSenderService>>().Object, new Mock<ITelemetryClient>().Object);
        }

        [TestMethod]
        public async Task StopService_ShouldCloseOnAllTopicClients()
        {
            await _dut.StopService();
            _topicClientMock1.Verify(t => t.CloseAsync(), Times.Once);
            _topicClientMock2.Verify(t => t.CloseAsync(), Times.Once);
            _topicClientMock3.Verify(t => t.CloseAsync(), Times.Once);
        }

        [TestMethod]
        public async Task SendMessageChunk_ShouldSendOnCorrectTopicClients()
        {
            Assert.AreEqual(Status.UnProcessed, _busEvents[0].Sent);
            Assert.AreEqual(Status.UnProcessed, _busEvents[0].Sent);

            await _dut.SendMessageChunk();
            _topicClientMock1.Verify(t => t.SendAsync(It.IsAny<Message>()),Times.Never);
            _topicClientMock2.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Once);
            _topicClientMock3.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Once);

            Assert.AreEqual(Status.Sent, _busEvents[0].Sent);
            Assert.AreEqual(Status.Sent, _busEvents[0].Sent);
        }

        [TestMethod]
        public async Task SendMessageChunk_ShouldSaveChangesAfterEachSend()
        {
            await _dut.SendMessageChunk();
            _iUnitOfWork.Verify(t => t.SaveChangesAsync(), Times.Between(2,2,Range.Inclusive));
        }
    }
}
