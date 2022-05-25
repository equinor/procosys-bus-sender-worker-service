using Equinor.ProCoSys.BusSenderWorker.Core;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Tests;

[TestClass]
public class BusEventRepositoryTests : RepositoryTestBase
{
    private BusEventRepository _dut;
    private List<BusEvent> _busEvents;
    private Mock<DbSet<BusEvent>> _busEventSetMock;
    private BusEvent _earliestEvent;
    private BusEvent _latestEvent;
    private BusEvent _secondToLatestEvent;

    [TestInitialize]
    public void Setup()
    {
        _earliestEvent = new BusEvent { Created = DateTime.Now.AddMinutes(-105), Event = "T", Sent = Status.UnProcessed, Id = 7, Message = "Message 105 minutes ago not sent" };
        _latestEvent = new BusEvent { Created = DateTime.Now.AddMinutes(-1), Event = "T", Sent = Status.UnProcessed, Id = 2, Message = "Message 1 minutes ago not sent" };
        _secondToLatestEvent = new BusEvent { Created = DateTime.Now.AddMinutes(-2), Event = "T", Sent = Status.UnProcessed, Id = 3, Message = "Message 2 minutes ago not sent" };

        _busEvents = new List<BusEvent>
        {
            new BusEvent { Created = DateTime.Now.AddMinutes(-10), Event = "T", Sent = Status.UnProcessed, Id = 1, Message = "Message 10 minutes ago not sent" },
            _latestEvent,
            _secondToLatestEvent,
            new BusEvent { Created = DateTime.Now.AddMinutes(-100), Event = "T", Sent = Status.UnProcessed, Id = 4, Message = "Message 100 minutes ago not sent" },
            new BusEvent { Created = DateTime.Now.AddMinutes(-80), Event = "T", Sent = Status.Failed, Id = 5, Message = "Message 80 minutes ago sent" },
            new BusEvent { Created = DateTime.Now.AddMinutes(-30), Event = "T", Sent = Status.UnProcessed, Id = 6, Message = "Message 30 minutes ago not sent" },
            _earliestEvent,
        };

        _busEventSetMock = _busEvents.AsQueryable().BuildMockDbSet();

        ContextHelper
            .ContextMock
            .Setup(x => x.BusEvents)
            .Returns(_busEventSetMock.Object);

        var configuration = new Mock<IConfiguration>();
        configuration.Setup(c => c["MessageChunkSize"]).Returns("5");
        _dut = new BusEventRepository(ContextHelper.ContextMock.Object, configuration.Object);
    }

    [TestMethod]
    public void GetEarliestUnProcessedEventChunk_ShouldReturnCorrectItemSequenceAndNumberOfItems()
    {
        var result = _dut.GetEarliestUnProcessedEventChunk();
        Assert.AreEqual(5, result.Result.Count);
        Assert.AreEqual(_earliestEvent, result.Result[0]);
        Assert.AreEqual(_secondToLatestEvent, result.Result[4]);
    }
}