using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Equinor.ProCoSys.BusSenderWorker.Core.Telemetry;
using Equinor.ProCoSys.PcsServiceBus.Sender;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Range = Moq.Range;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;

[TestClass]
public class BusSenderServiceTests
{
    private BusSenderService _dut;
    private Mock<IUnitOfWork> _iUnitOfWork;
    private Mock<ServiceBusSender> _topicClientMock1, _topicClientMock2, _topicClientMock3, _topicClientMock4, _topicClientMockWo;
    private List<BusEvent> _busEvents;
    private Mock<IBusEventRepository> _busEventRepository;
    private Mock<ITagDetailsRepository> _tagDetailsRepositoryMock;
    private Mock<IBusSenderMessageRepository> _busSenderMessageRepositoryMock;
    private Mock<BusEventService> _busEventServiceMock;
    private List<ServiceBusMessage> _messagesInTopicClient4;

    [TestInitialize]
    public void Setup()
    {
        var busSender = new PcsBusSender();
        _topicClientMock1 = new Mock<ServiceBusSender>();
        _topicClientMock1.Setup(t => t.CreateMessageBatchAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => ServiceBusModelFactory.ServiceBusMessageBatch(1000, new List<ServiceBusMessage>()));
        _topicClientMock2 = new Mock<ServiceBusSender>();
        _topicClientMock2.Setup(t => t.CreateMessageBatchAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => ServiceBusModelFactory.ServiceBusMessageBatch(1000, new List<ServiceBusMessage>()));
        _topicClientMock3 = new Mock<ServiceBusSender>();
        _topicClientMock3.Setup(t => t.CreateMessageBatchAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => ServiceBusModelFactory.ServiceBusMessageBatch(1000, new List<ServiceBusMessage>()));
        _topicClientMock4 = new Mock<ServiceBusSender>();
        _topicClientMockWo = new Mock<ServiceBusSender>();
        _messagesInTopicClient4  = new List<ServiceBusMessage>();
        _topicClientMockWo.Setup(t => t.CreateMessageBatchAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => ServiceBusModelFactory.ServiceBusMessageBatch(1000, new List<ServiceBusMessage>()));
        _topicClientMock4.Setup(t => t.CreateMessageBatchAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(()=> ServiceBusModelFactory.ServiceBusMessageBatch(1000, _messagesInTopicClient4));
        busSender.Add("topic1", _topicClientMock1.Object);
        busSender.Add("topic2", _topicClientMock2.Object);
        busSender.Add("topic3", _topicClientMock3.Object);
        busSender.Add("topic4", _topicClientMock4.Object);
        busSender.Add(WorkOrderTopic.TopicName, _topicClientMockWo.Object);

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
        _dut = new BusSenderService(busSender, _busEventRepository.Object, _iUnitOfWork.Object, new Mock<ILogger<BusSenderService>>().Object,
            new Mock<ITelemetryClient>().Object, _busEventServiceMock.Object);
    }

    [TestMethod]
    public async Task StopService_ShouldCloseOnAllTopicClients()
    {
        await _dut.CloseConnections();
        _topicClientMock1.Verify(t => t.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
        _topicClientMock2.Verify(t => t.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
        _topicClientMock3.Verify(t => t.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
        _topicClientMock4.Verify(t => t.CloseAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestMethod]
    public async Task SendMessageChunk_ShouldSendOnCorrectTopicClients()
    {
        Assert.AreEqual(Status.UnProcessed, _busEvents[0].Status);
        Assert.AreEqual(Status.UnProcessed, _busEvents[0].Status);

        await _dut.HandleBusEvents();
        _topicClientMock1.Verify(t => t.SendMessagesAsync(It.IsAny<ServiceBusMessageBatch>(), It.IsAny<CancellationToken>()), Times.Never);
        _topicClientMock2.Verify(t => t.SendMessagesAsync(It.IsAny<ServiceBusMessageBatch>(), It.IsAny<CancellationToken>()), Times.Once);
        _topicClientMock3.Verify(t => t.SendMessagesAsync(It.IsAny<ServiceBusMessageBatch>(), It.IsAny<CancellationToken>()), Times.Once);
        _topicClientMock4.Verify(t => t.SendMessagesAsync(It.IsAny<ServiceBusMessageBatch>(), It.IsAny<CancellationToken>()), Times.Never);

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

        Assert.AreEqual(expectedWashedMessage, Encoding.UTF8.GetString(_messagesInTopicClient4[0].Body));
    }

    [TestMethod]
    public async Task SendMessageChunk_ShouldSaveChangesAfterEachSend()
    {
        await _dut.HandleBusEvents();
        _iUnitOfWork.Verify(t => t.SaveChangesAsync(), Times.Between(2, 3, Range.Inclusive));
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
        _topicClientMockWo.Verify(t => t.SendMessagesAsync(It.IsAny<ServiceBusMessageBatch>(), It.IsAny<CancellationToken>()), Times.Once);
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
        _topicClientMockWo.Verify(t => t.SendMessagesAsync(It.IsAny<ServiceBusMessageBatch>(), It.IsAny<CancellationToken>()), Times.Exactly(1));
    }
}