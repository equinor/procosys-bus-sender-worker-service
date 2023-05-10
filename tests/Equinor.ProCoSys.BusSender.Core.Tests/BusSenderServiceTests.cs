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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Equinor.ProCoSys.PcsServiceBus.Queries;
using Equinor.ProCoSys.PcsServiceBus.Topics;
using Range = Moq.Range;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;

[TestClass]
public class BusSenderServiceTests
{
    private Mock<IBusEventRepository> _busEventRepository;
    private List<BusEvent> _busEvents;
    private Mock<BusEventService> _busEventServiceMock;
    private PcsBusSender _busSender;
    private Mock<IEventRepository> _dapperRepositoryMock;
    private BusSenderService _dut;
    private Mock<IUnitOfWork> _iUnitOfWork;
    private List<ServiceBusMessage> _messagesInTopicClient4;
    private ServiceBusMessageBatch _mockWoMessageBatch;
    private Mock<ITagDetailsRepository> _tagDetailsRepositoryMock;

    private Mock<ServiceBusSender> _topicClientMock1,
        _topicClientMock2,
        _topicClientMock3,
        _topicClientMock4,
        _topicClientMockWo;

    [TestMethod]
    public async Task HandleBusEvents_ShouldAddMessagesToBatchInCorrectOrder()
    {
        //Arrange
        const string deleteMessage =
            "{\"Plant\" : \"AnyValidPlant\", \"ProCoSysGuid\" : \"someGuid\", \"Behavior\" : \"delete\"}";
        var wcl1 = new BusEvent
            { Event = WoChecklistTopic.TopicName, Message = "10001,1003", Status = Status.UnProcessed };
        var wcl2 = new BusEvent
            { Event = WoChecklistTopic.TopicName, Message = "10001,1003", Status = Status.UnProcessed };
        var wcl3 = new BusEvent
            { Event = WoChecklistTopic.TopicName, Message = "1003,10001", Status = Status.UnProcessed };
        var wcDelete = new BusEvent
            { Event = WoChecklistTopic.TopicName, Message = deleteMessage, Status = Status.UnProcessed };
        var workOrderChecklistEvent1 = new WorkOrderChecklistEvent { WoNo = "SomeWoNo1" };
        var workOrderChecklistEvent2 = new WorkOrderChecklistEvent { WoNo = "SomeWoNo2" };

        _busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk())
            .Returns(() => Task.FromResult(new List<BusEvent> { wcl1, wcl2, wcl3, wcDelete }));

        _dapperRepositoryMock.Setup(wcl => wcl.QuerySingle<WorkOrderChecklistEvent>(It.IsAny<string>(), "10001,1003"))
            .Returns(() => Task.FromResult(workOrderChecklistEvent1));
        _dapperRepositoryMock.Setup(wcl => wcl.QuerySingle<WorkOrderChecklistEvent>(It.IsAny<string>(), "1003,10001"))
            .Returns(() => Task.FromResult(workOrderChecklistEvent2));

        var topicClientMockWoCl = new Mock<ServiceBusSender>();
        _busSender.Add(WoChecklistTopic.TopicName, topicClientMockWoCl.Object);

        var serviceBusMessages = new List<ServiceBusMessage>();
        var batch = ServiceBusModelFactory.ServiceBusMessageBatch(1000, serviceBusMessages);

        topicClientMockWoCl.Setup(t => t.CreateMessageBatchAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => batch);

        //Act
        await _dut.HandleBusEvents();

        //Assert
        topicClientMockWoCl.Verify(t => t.SendMessagesAsync(batch, It.IsAny<CancellationToken>()), Times.Exactly(1));
        Assert.AreEqual(3, batch.Count);
        Assert.AreEqual(3, serviceBusMessages.Count);
        Assert.AreEqual(JsonSerializer.Serialize(workOrderChecklistEvent1), serviceBusMessages[0].Body.ToString());
        Assert.AreEqual(JsonSerializer.Serialize(workOrderChecklistEvent2), serviceBusMessages[1].Body.ToString());
        Assert.AreEqual(deleteMessage, serviceBusMessages[2].Body.ToString());
    }

    [TestMethod]
    public async Task HandleBusEvents_ShouldHandleCompositeSimpleMessages()
    {
        //Arrange
        var wcl1 = new BusEvent
            { Event = WoChecklistTopic.TopicName, Message = "10001,1003", Status = Status.UnProcessed };
        var wcl2 = new BusEvent
            { Event = WoChecklistTopic.TopicName, Message = "10001,1003", Status = Status.UnProcessed };
        var wcl3 = new BusEvent
            { Event = WoChecklistTopic.TopicName, Message = "1003,10001", Status = Status.UnProcessed };

        _busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk())
            .Returns(() => Task.FromResult(new List<BusEvent> { wcl1, wcl2, wcl3 }));

        var topicClientMockWoCl = new Mock<ServiceBusSender>();
        _busSender.Add(WoChecklistTopic.TopicName, topicClientMockWoCl.Object);
        var woClMessageBatch = ServiceBusModelFactory.ServiceBusMessageBatch(1000, new List<ServiceBusMessage>());
        topicClientMockWoCl.Setup(t => t.CreateMessageBatchAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => woClMessageBatch);

        //Act
        await _dut.HandleBusEvents();

        //Assert
        topicClientMockWoCl.Verify(t => t.SendMessagesAsync(woClMessageBatch, It.IsAny<CancellationToken>()),
            Times.Exactly(1));
        Assert.AreEqual(2, woClMessageBatch.Count);
    }

    [TestMethod]
    public async Task HandleBusEvents_ShouldHandleGuidSimpleMessages()
    {
        //Arrange
        const string guid = "E9414BA9930E5FF6E0532510000AA1AB";
        var commPri = new BusEvent { Event = LibraryFieldTopic.TopicName, Message = guid, Status = Status.UnProcessed };

        _busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk())
            .Returns(() => Task.FromResult(new List<BusEvent> { commPri }));

        var topicClientMock = new Mock<ServiceBusSender>();
        _busSender.Add(LibraryFieldTopic.TopicName, topicClientMock.Object);
        var batch = ServiceBusModelFactory.ServiceBusMessageBatch(1000, new List<ServiceBusMessage>());
        topicClientMock.Setup(t => t.CreateMessageBatchAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => batch);

        //Act
        await _dut.HandleBusEvents();

        //Assert
        topicClientMock.Verify(t => t.SendMessagesAsync(batch, It.IsAny<CancellationToken>()), Times.Exactly(1));
        Assert.AreEqual(1, batch.Count);
    }

    [TestMethod]
    public async Task HandleBusEvents_ShouldHandleInvalidCharacters()
    {
        //Arrange
        var wo1 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10000", Status = Status.UnProcessed };
        var wo2 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10000", Status = Status.UnProcessed };
        var wo3 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10001", Status = Status.UnProcessed };

        var wo = new WorkOrderEvent()
            { Plant = "AnyValidPlant", ProjectName = "AnyProjectName", WoNo = "Some0x0bWoNo" };

        _busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk())
            .Returns(() => Task.FromResult(new List<BusEvent> { wo1, wo2, wo3 }));
        _dapperRepositoryMock.Setup(wr => wr.QuerySingle<WorkOrderEvent>(It.IsAny<string>(), "10000"))
            .Returns(() => Task.FromResult(wo));
        _dapperRepositoryMock.Setup(wr => wr.QuerySingle<WorkOrderEvent>(It.IsAny<string>(), "10001"))
            .Returns(() => Task.FromResult(wo));

        //Act
        await _dut.HandleBusEvents();

        //Assert
        _topicClientMockWo.Verify(
            t => t.SendMessagesAsync(It.IsAny<ServiceBusMessageBatch>(), It.IsAny<CancellationToken>()),
            Times.Exactly(1));
    }

    [TestMethod]
    public async Task HandleBusEvents_ShouldSendCorrectAmountOfMessages()
    {
        //Arrange
        var wo1 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10000", Status = Status.UnProcessed };
        var wo2 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10000", Status = Status.UnProcessed };
        var wo3 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10001", Status = Status.UnProcessed };

        var wo = new WorkOrderEvent()
            { Plant = "AnyValidPlant", ProjectName = "AnyProjectName", WoNo = "SomeWoNo" };


        _busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk())
            .Returns(() => Task.FromResult(new List<BusEvent> { wo1, wo2, wo3 }));
        _dapperRepositoryMock.Setup(wr => wr.QuerySingle<WorkOrderEvent>(It.IsAny<string>(), wo1.ToString()))
            .Returns(() => Task.FromResult(wo));
        _dapperRepositoryMock.Setup(wr => wr.QuerySingle<WorkOrderEvent>(It.IsAny<string>(), wo2.ToString()))
            .Returns(() => Task.FromResult(wo));
        _dapperRepositoryMock.Setup(wr => wr.QuerySingle<WorkOrderEvent>(It.IsAny<string>(), wo3.ToString()))
            .Returns(() => Task.FromResult(wo));

        //Act
        await _dut.HandleBusEvents();

        //Assert
        _topicClientMockWo.Verify(t => t.SendMessagesAsync(_mockWoMessageBatch, It.IsAny<CancellationToken>()),
            Times.Exactly(1));
        Assert.AreEqual(2, _mockWoMessageBatch.Count);
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
    public async Task SendMessageChunk_ShouldSendOnCorrectTopicClients()
    {
        Assert.AreEqual(Status.UnProcessed, _busEvents[0].Status);
        Assert.AreEqual(Status.UnProcessed, _busEvents[0].Status);

        await _dut.HandleBusEvents();
        _topicClientMock1.Verify(
            t => t.SendMessagesAsync(It.IsAny<ServiceBusMessageBatch>(), It.IsAny<CancellationToken>()), Times.Never);
        _topicClientMock2.Verify(
            t => t.SendMessagesAsync(It.IsAny<ServiceBusMessageBatch>(), It.IsAny<CancellationToken>()), Times.Once);
        _topicClientMock3.Verify(
            t => t.SendMessagesAsync(It.IsAny<ServiceBusMessageBatch>(), It.IsAny<CancellationToken>()), Times.Once);
        _topicClientMock4.Verify(
            t => t.SendMessagesAsync(It.IsAny<ServiceBusMessageBatch>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.AreEqual(Status.Sent, _busEvents[0].Status);
        Assert.AreEqual(Status.Sent, _busEvents[0].Status);
    }

    [TestMethod]
    public async Task SendMessageChunk_ShouldSendOnlyOneMessageForDuplicates()
    {
        //Arrange
        var wo1 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10000", Status = Status.UnProcessed };
        var wo2 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10000", Status = Status.UnProcessed };
        var wo3 = new BusEvent { Event = WorkOrderTopic.TopicName, Message = "10000", Status = Status.UnProcessed };

        var wo = new WorkOrderEvent()
            { Plant = "AnyValidPlant", ProjectName = "AnyProjectName", WoNo = "SomeWoNo" };


        _busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk())
            .Returns(() => Task.FromResult(new List<BusEvent> { wo1, wo2, wo3 }));
        _dapperRepositoryMock.Setup(wr => wr.QuerySingle<WorkOrderEvent>(It.IsAny<string>(), wo1.ToString()))
            .Returns(() => Task.FromResult(wo));
        _dapperRepositoryMock.Setup(wr => wr.QuerySingle<WorkOrderEvent>(It.IsAny<string>(), wo2.ToString()))
            .Returns(() => Task.FromResult(wo));
        _dapperRepositoryMock.Setup(wr => wr.QuerySingle<WorkOrderEvent>(It.IsAny<string>(), wo3.ToString()))
            .Returns(() => Task.FromResult(wo));

        //Act
        await _dut.HandleBusEvents();

        //Assert
        _dapperRepositoryMock.Verify(dr => dr.QuerySingle<WorkOrderEvent>(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);
        _topicClientMockWo.Verify(
            t => t.SendMessagesAsync(It.IsAny<ServiceBusMessageBatch>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [TestInitialize]
    public void Setup()
    {
        _busSender = new PcsBusSender();
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
        _messagesInTopicClient4 = new List<ServiceBusMessage>();
        _mockWoMessageBatch = ServiceBusModelFactory.ServiceBusMessageBatch(1000, new List<ServiceBusMessage>());
        _topicClientMockWo.Setup(t => t.CreateMessageBatchAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => _mockWoMessageBatch);
        _topicClientMock4.Setup(t => t.CreateMessageBatchAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => ServiceBusModelFactory.ServiceBusMessageBatch(1000, _messagesInTopicClient4));
        _busSender.Add("topic1", _topicClientMock1.Object);
        _busSender.Add("topic2", _topicClientMock2.Object);
        _busSender.Add("topic3", _topicClientMock3.Object);
        _busSender.Add("topic4", _topicClientMock4.Object);
        _busSender.Add(WorkOrderTopic.TopicName, _topicClientMockWo.Object);


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
                Message =
                    "{\"Plant\" : \"PCS$HF_LNG\", \"Responsible\" : \"8460-E015\", \"Description\" : \"	Installere bonding til JBer ved V8 område\"}"
            }
        };
        _busEventRepository = new Mock<IBusEventRepository>();
        _tagDetailsRepositoryMock = new Mock<ITagDetailsRepository>();

        _dapperRepositoryMock = new Mock<IEventRepository>();
        _busEventServiceMock = new Mock<BusEventService>(_tagDetailsRepositoryMock.Object, _dapperRepositoryMock.Object)
            { CallBase = true };
        _iUnitOfWork = new Mock<IUnitOfWork>();

        _busEventRepository.Setup(b => b.GetEarliestUnProcessedEventChunk()).Returns(() => Task.FromResult(_busEvents));
        _dut = new BusSenderService(_busSender, _busEventRepository.Object, _iUnitOfWork.Object,
            new Mock<ILogger<BusSenderService>>().Object,
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
}