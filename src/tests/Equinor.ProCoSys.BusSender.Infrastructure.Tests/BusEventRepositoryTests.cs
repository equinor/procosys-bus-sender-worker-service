using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.BusSender.Core.Models;
using Equinor.ProCoSys.BusSender.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;

namespace Equinor.ProCoSys.BusSender.Infrastructure.Tests
{
    [TestClass]
    public class BusEventRepositoryTests : RepositoryTestBase
    {
        private BusEventRepository _dut;
        private List<BusEvent> _busEvents;
        private Mock<DbSet<BusEvent>> _busEventSetMock;
        private BusEvent _earliesEvent;
        private BusEvent _latestEvent;
        [TestInitialize]
        public void Setup()
        {
            _earliesEvent = new BusEvent { Created = DateTime.Now.AddMinutes(-105), Event = "T", Sent = 0, Id = 7, Message = "Message 105 minutes ago not sent" };
            _latestEvent = new BusEvent { Created = DateTime.Now.AddMinutes(-1), Event = "T", Sent = 0, Id = 2, Message = "Message 1 minutes ago not sent" };

            _busEvents = new List<BusEvent>
            {
                new BusEvent { Created = DateTime.Now.AddMinutes(-10), Event = "T", Sent = 0, Id = 1, Message = "Message 10 minutes ago not sent" },
                _latestEvent,
                new BusEvent { Created = DateTime.Now.AddMinutes(-2), Event = "T", Sent = 0, Id = 3, Message = "Message 2 minutes ago not sent" },
                new BusEvent { Created = DateTime.Now.AddMinutes(-100), Event = "T", Sent = 0, Id = 4, Message = "Message 100 minutes ago not sent" },
                new BusEvent { Created = DateTime.Now.AddMinutes(-80), Event = "T", Sent = 2, Id = 5, Message = "Message 80 minutes ago sent" },
                new BusEvent { Created = DateTime.Now.AddMinutes(-30), Event = "T", Sent = 0, Id = 6, Message = "Message 30 minutes ago not sent" },
                _earliesEvent,
            };

            _busEventSetMock = _busEvents.AsQueryable().BuildMockDbSet();

            ContextHelper
                .ContextMock
                .Setup(x => x.BusEvents)
                .Returns(_busEventSetMock.Object);

            _dut = new BusEventRepository(ContextHelper.ContextMock.Object);
        }

        [TestMethod]
        public void GetEarliestUnProcessedEventChunk()
        {
            var result = _dut.GetEarliestUnProcessedEventChunk();
            Assert.AreEqual(6, result.Result.Count);
            Assert.AreEqual(_earliesEvent, result.Result[0]);
            Assert.AreEqual(_latestEvent, result.Result[5]);
        }
    }
}
