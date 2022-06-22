﻿using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.PcsServiceBus.Sender;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Range = Moq.Range;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;

[TestClass]
public class BusSenderServiceTests
{
    private BusSenderService _dut;
    private Mock<IUnitOfWork> _iUnitOfWork;
    private Mock<ITopicClient> _topicClientMock1, _topicClientMock2, _topicClientMock3, _topicClientMock4, _topicClientMockWo;
    private List<BusEvent> _busEvents;
    private Mock<IBusEventRepository> _busEventRepository;
    private Mock<ITagDetailsRepository> _tagDetailsRepositoryMock;
    private Mock<IBusSenderMessageRepository> _busSenderMessageRepositoryMock;
    private Mock<BusEventService> _busEventServiceMock;
    private string _messageBodyOnTopicClient4;

    [TestInitialize]
    public void Setup()
    {
        var topicClients = new PcsBusSender();
        _topicClientMock1 = new Mock<ITopicClient>();
        _topicClientMock2 = new Mock<ITopicClient>();
        _topicClientMock3 = new Mock<ITopicClient>();
        _topicClientMock4 = new Mock<ITopicClient>();
        _topicClientMockWo = new Mock<ITopicClient>();
        _topicClientMock4.Setup(t => t.SendAsync(It.IsAny<Message>()))
            .Callback<Message>(m => _messageBodyOnTopicClient4 = Encoding.UTF8.GetString(m.Body));
        topicClients.Add("topic1", _topicClientMock1.Object);
        topicClients.Add("topic2", _topicClientMock2.Object);
        topicClients.Add("topic3", _topicClientMock3.Object);
        topicClients.Add("topic4", _topicClientMock4.Object);
        topicClients.Add(WorkOrderTopic.TopicName, _topicClientMockWo.Object);

        _busEvents = new List<BusEvent>
        {
            new()
            {
                Created = DateTime.Now.AddMinutes(-10),
                Event = "topic2",
                Status = Status.UnProcessed,
                Id = 1,
                Message = "{\"Plant\":\"NGPCS_TEST_BROWN\",\"ProjectName\":\"Message 10 minutes ago not sent\"}"
            },
            new()
            {
                Created = DateTime.Now.AddMinutes(-10),
                Event = "topic3",
                Status = Status.UnProcessed,
                Id = 1,
                Message = "{\"Plant\" : \"PCS$HF_LNG\", \"Responsible\" : \"8460-E015\", \"Description\" : \"	Installere bonding til JBer ved V8 område\"}"
            }
        };
        _busEventRepository = new Mock<IBusEventRepository>();
        _tagDetailsRepositoryMock = new Mock<ITagDetailsRepository>();
        
        _busSenderMessageRepositoryMock = new Mock<IBusSenderMessageRepository>();
        _busEventServiceMock = new Mock<BusEventService>(_tagDetailsRepositoryMock.Object, _busSenderMessageRepositoryMock.Object) { CallBase = true };
        _iUnitOfWork = new Mock<IUnitOfWork>();

        _busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk()).Returns(() => Task.FromResult(_busEvents));
        _dut = new BusSenderService(topicClients, _busEventRepository.Object, _iUnitOfWork.Object, new Mock<ILogger<BusSenderService>>().Object,
            new Mock<ITelemetryClient>().Object, _busEventServiceMock.Object);
    }

    [TestMethod]
    public async Task StopService_ShouldCloseOnAllTopicClients()
    {
        await _dut.CloseConnections();
        _topicClientMock1.Verify(t => t.CloseAsync(), Times.Once);
        _topicClientMock2.Verify(t => t.CloseAsync(), Times.Once);
        _topicClientMock3.Verify(t => t.CloseAsync(), Times.Once);
        _topicClientMock4.Verify(t => t.CloseAsync(), Times.Once);
    }

    [TestMethod]
    public async Task SendMessageChunk_ShouldSendOnCorrectTopicClients()
    {
        Assert.AreEqual(Status.UnProcessed, _busEvents[0].Status);
        Assert.AreEqual(Status.UnProcessed, _busEvents[0].Status);

        await _dut.HandleBusEvents();
        _topicClientMock1.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Never);
        _topicClientMock2.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Once);
        _topicClientMock3.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Once);
        _topicClientMock4.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Never);

        Assert.AreEqual(Status.Sent, _busEvents[0].Status);
        Assert.AreEqual(Status.Sent, _busEvents[0].Status);
    }

    [TestMethod]
    public async Task SendMessageChunk_ShouldCleanMessageJSon()
    {
        var uncleanedTestMessage = new BusEvent
        {
            Event = "topic4",
            Message =
                "{\"Plant\" : \"PCS$HF_LNG\",\"Parameter\":\"En \t\r\n\fstreng\" }"
        };
        const string expectedWashedMessage = "{\"Plant\" : \"PCS$HF_LNG\",\"Parameter\":\"En streng\" }";

        _busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk())
            .Returns(() => Task.FromResult(new List<BusEvent> { uncleanedTestMessage }));

        await _dut.HandleBusEvents();

        Assert.AreEqual(expectedWashedMessage, _messageBodyOnTopicClient4);
    }

    [TestMethod]
    public async Task SendMessageChunk_ShouldSaveChangesAfterEachSend()
    {
        await _dut.HandleBusEvents();
        _iUnitOfWork.Verify(t => t.SaveChangesAsync(), Times.Between(2, 2, Range.Inclusive));
    }

    [TestMethod]
    public async Task SendMessageChunk_ShouldSendOnlyOneMessageForDuplicates()
    {
        //Arrange
        var wo1 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10000", Status = Status.UnProcessed };
        var wo2 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10000", Status = Status.UnProcessed };
        var wo3 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10000", Status = Status.UnProcessed };

        const string jsonMessage =
            "{\"Plant\" : \"AnyValidPlant\", \"ProjectName\" : \"AnyProjectName\", \"WoNo\" : \"SomeWoNo\"}";

        _busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk())
            .Returns(() => Task.FromResult(new List<BusEvent> { wo1, wo2, wo3 }));
        _busSenderMessageRepositoryMock.Setup(wr => wr.GetWorkOrderMessage(10000))
            .Returns(() => Task.FromResult(jsonMessage));

        //Act
        await _dut.HandleBusEvents();

        //Assert
        _topicClientMockWo.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Once);
    }

    [TestMethod]
    public async Task HandleBusEvents_ShouldSendCorrectAmountOfMessages()
    {
        //Arrange
        var wo1 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10000", Status = Status.UnProcessed };
        var wo2 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10000", Status = Status.UnProcessed };
        var wo3 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10001", Status = Status.UnProcessed };

        const string jsonMessage =
            "{\"Plant\" : \"AnyValidPlant\", \"ProjectName\" : \"AnyProjectName\", \"WoNo\" : \"SomeWoNo\"}";

        _busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk())
            .Returns(() => Task.FromResult(new List<BusEvent> { wo1, wo2, wo3 }));
        _busSenderMessageRepositoryMock.Setup(wr => wr.GetWorkOrderMessage(10000))
            .Returns(() => Task.FromResult(jsonMessage));
        _busSenderMessageRepositoryMock.Setup(wr => wr.GetWorkOrderMessage(10001))
            .Returns(() => Task.FromResult(jsonMessage));

        //Act
        await _dut.HandleBusEvents();

        //Assert
        _topicClientMockWo.Verify(t => t.SendAsync(It.IsAny<Message>()), Times.Exactly(2));
    }
}