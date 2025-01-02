using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Equinor.ProCoSys.BusSenderWorker.Core;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Equinor.ProCoSys.BusSenderWorker.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MockQueryable.Moq;
using Moq;
using Moq.Protected;

namespace Equinor.ProCoSys.BusSenderWorker.Infrastructure.Tests;

[TestClass]
public class BusEventRepositoryTests : RepositoryTestBase
{
    private List<BusEvent> _busEvents;
    private Mock<DbSet<BusEvent>> _busEventSetMock;
    private BusEventRepository _dut;
    private BusEvent _earliestEvent;
    private BusEvent _latestEvent;
    private BusEvent _secondToLatestEvent;

    [TestMethod]
    public void GetEarliestUnProcessedEventChunk_ShouldReturnCorrectItemSequenceAndNumberOfItems()
    {
        var configuration = new Mock<IPlantService>();
        configuration.Setup(c => c.GetConfiguration()["MessageChunkSize"]).Returns("5");
        configuration.Setup(c => c.GetPlantsHandledByCurrentInstance()).Returns(new List<string>() { "PCS$Plant" });

        var dut = new BusEventRepository(ContextHelper.ContextMock.Object, configuration.Object);

        Task<List<BusEvent>> result = dut.GetEarliestUnProcessedEventChunk();

        Assert.AreEqual(5, result.Result.Count);
        Assert.AreEqual(_earliestEvent, result.Result[0]);
        Assert.AreEqual(_secondToLatestEvent, result.Result[4]);
    }

    [TestMethod]
    public void GetEarliestUnProcessedEventChunk_WhenPlantConstant_ShouldReturnItemsForAllPlantsExceptNullPlants()
    {
        // Arrange
        var serviceAEarliestEvent = new BusEvent
        {
            Created = DateTime.Now.AddMinutes(-200),
            Event = "T",
            Status = Status.UnProcessed,
            Id = 8,
            Plant = "PCS$PlantA",
            Message = "Message 200 minutes ago not sent"
        };
        var serviceABusEvents = new List<BusEvent>
        {
            serviceAEarliestEvent,
            new()
            {
                Created = DateTime.Now.AddMinutes(-150), Event = "T", Status = Status.UnProcessed, Id = 9, Plant = "PCS$PlantB",
                Message = "Message 150 minutes ago not sent"
            },
            new()
            {
                Created = DateTime.Now.AddMinutes(-50), Event = "T", Status = Status.UnProcessed, Id = 10, Plant = "PCS$PlantC",
                Message = "Message 50 minutes ago not sent"
            },
            new()
            {
                Created = DateTime.Now.AddMinutes(-50), Event = "T", Status = Status.UnProcessed, Id = 10, Plant = null,
                Message = "Message 50 minutes ago not sent"
            }
        };

        var serviceABusEventSetMock = serviceABusEvents.AsQueryable().BuildMockDbSet();

        ContextHelper
            .ContextMock
            .Setup(x => x.BusEvents)
            .Returns(serviceABusEventSetMock.Object);

        var serviceAConfiguration = new Mock<IPlantService>();
        serviceAConfiguration.Setup(c => c.GetConfiguration()["MessageChunkSize"]).Returns("3");
        serviceAConfiguration.Setup(c => c.GetPlantsHandledByCurrentInstance()).Returns(new List<string>() { "PLANT" });

        var serviceADut = new BusEventRepository(ContextHelper.ContextMock.Object, serviceAConfiguration.Object);

        var result = serviceADut.GetEarliestUnProcessedEventChunk();

        foreach (var busEvent in result.Result)
        {
            Assert.IsTrue(busEvent.Plant == "PCS$PlantA" || busEvent.Plant == "PCS$PlantB" || busEvent.Plant == "PCS$PlantC");
        }
    }

    [TestMethod]
    public void GetEarliestUnProcessedEventChunk_WithDifferentInstance_ShouldReturnItemsForAssignedPlantsOnly()
    {
        // Arrange
        var serviceAEarliestEvent = new BusEvent
        {
            Created = DateTime.Now.AddMinutes(-200),
            Event = "T",
            Status = Status.UnProcessed,
            Id = 8,
            Plant = "PCS$PlantA",
            Message = "Message 200 minutes ago not sent"
        };
        var serviceABusEvents = new List<BusEvent>
        {
            serviceAEarliestEvent,
            new()
            {
                Created = DateTime.Now.AddMinutes(-150), Event = "T", Status = Status.UnProcessed, Id = 9, Plant = "PCS$PlantB",
                Message = "Message 150 minutes ago not sent"
            },
            new()
            {
                Created = DateTime.Now.AddMinutes(-50), Event = "T", Status = Status.UnProcessed, Id = 10, Plant = "PCS$PlantC",
                Message = "Message 50 minutes ago not sent"
            },
            new()
            {
                Created = DateTime.Now.AddMinutes(-50), Event = "T", Status = Status.UnProcessed, Id = 10, Plant = null,
                Message = "Message 50 minutes ago not sent"
            }
        };

        var serviceABusEventSetMock = serviceABusEvents.AsQueryable().BuildMockDbSet();

        ContextHelper
            .ContextMock
            .Setup(x => x.BusEvents)
            .Returns(serviceABusEventSetMock.Object);

        var serviceAConfiguration = new Mock<IPlantService>();
        serviceAConfiguration.Setup(c => c.GetConfiguration()["MessageChunkSize"]).Returns("3");
        serviceAConfiguration.Setup(c => c.GetPlantsHandledByCurrentInstance()).Returns(new List<string>() { "PCS$PlantA", "PCS$PlantB", "NOPLANT" });

        var serviceADut = new BusEventRepository(ContextHelper.ContextMock.Object, serviceAConfiguration.Object);

        var result = serviceADut.GetEarliestUnProcessedEventChunk();

        foreach (var busEvent in result.Result)
        {
            Assert.IsTrue(busEvent.Plant == "PCS$PlantA" || busEvent.Plant == "PCS$PlantB" || busEvent.Plant == null);
        }
    }


    [TestMethod]
    public void GetEarliestUnProcessedEventChunk_WithInstanceHandlingRemaining_ShouldReturnItemsForAssignedAndRemainingPlantsOnly()
    {
        // Arrange
        var serviceAEarliestEvent = new BusEvent
        {
            Created = DateTime.Now.AddMinutes(-200),
            Event = "T",
            Status = Status.UnProcessed,
            Id = 8,
            Plant = "PCS$PlantA",
            Message = "Message 200 minutes ago not sent"
        };
        var differentBusEvents = new List<BusEvent>
        {
            serviceAEarliestEvent,
            new()
            {
                Created = DateTime.Now.AddMinutes(-150), Event = "T", Status = Status.UnProcessed, Id = 9, Plant = "PCS$PlantB",
                Message = "Message 150 minutes ago not sent"
            },
            new()
            {
                Created = DateTime.Now.AddMinutes(-50), Event = "T", Status = Status.UnProcessed, Id = 10, Plant = "PCS$PlantC",
                Message = "Message 50 minutes ago not sent"
            }
        };

        var differentBusEventSetMock = differentBusEvents.AsQueryable().BuildMockDbSet();

        ContextHelper
            .ContextMock
            .Setup(x => x.BusEvents)
            .Returns(differentBusEventSetMock.Object);

        var host = new Mock<IHost>();
        var plantsHandledByThisInstance = "PCS$PlantA,REMAININGPLANTS";
        var allPlants = new List<string> { "PCS$PlantA", "PCS$PlantB", "PCS$PlantC" };
        var plantsHandledByOtherInstances = new List<string> { "PCS$PlantC" };


        var mockLogger = new Mock<ILogger<PlantService>>();

        var configuration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
        configuration.Setup(c => c["MessageChunkSize"]).Returns("3");
        configuration.Setup(c => c["MessageSites"]).Returns(plantsHandledByThisInstance);
        configuration.Setup(c => c["InstanceName"]).Returns("ServiceA");

        var plantService = new Mock<PlantService>(configuration.Object, mockLogger.Object) { CallBase = true };
        // Using reflection to setup protected method
        plantService
            .Protected()
            .Setup<Task<List<string>>>("RegisterHandledPlantsAsync", ItExpr.IsAny<IHost>())
            .ReturnsAsync(plantsHandledByOtherInstances);

        plantService.Object.RegisterPlantsHandledByCurrentInstance(host.Object, allPlants);
        var serviceADut = new BusEventRepository(ContextHelper.ContextMock.Object, plantService.Object);
        var result = serviceADut.GetEarliestUnProcessedEventChunk();

        Assert.AreEqual(2, result.Result.Count);
        Assert.AreEqual(serviceAEarliestEvent, result.Result[0]);
        Assert.AreEqual(differentBusEvents[1], result.Result[1]);
    }


    [TestInitialize]
    public void Setup()
    {
        _earliestEvent = new BusEvent
        {
            Created = DateTime.Now.AddMinutes(-105), Event = "T", Status = Status.UnProcessed, Id = 1, Plant = "PCS$Plant",
            Message = "Message 105 minutes ago not sent"
        };
        _latestEvent = new BusEvent
        {
            Created = DateTime.Now.AddMinutes(-1), Event = "T", Status = Status.UnProcessed, Id = 7, Plant = "PCS$Plant",
            Message = "Message 1 minutes ago not sent"
        };
        _secondToLatestEvent = new BusEvent
        {
            Created = DateTime.Now.AddMinutes(-2), Event = "T", Status = Status.UnProcessed, Id = 6, Plant = "PCS$Plant",
            Message = "Message 2 minutes ago not sent"
        };

        _busEvents = new List<BusEvent>
        {
            new()
            {
                Created = DateTime.Now.AddMinutes(-10), Event = "T", Status = Status.UnProcessed, Id = 5, Plant = "PCS$Plant",
                Message = "Message 10 minutes ago not sent"
            },
            _latestEvent,
            _secondToLatestEvent,
            new()
            {
                Created = DateTime.Now.AddMinutes(-100), Event = "T", Status = Status.UnProcessed, Id = 2, Plant = "PCS$Plant",
                Message = "Message 100 minutes ago not sent"
            },
            new()
            {
                Created = DateTime.Now.AddMinutes(-80), Event = "T", Status = Status.Failed, Id = 3, Plant = "PCS$Plant",
                Message = "Message 80 minutes ago sent"
            },
            new()
            {
                Created = DateTime.Now.AddMinutes(-30), Event = "T", Status = Status.UnProcessed, Id = 4, Plant = "PCS$Plant",
                Message = "Message 30 minutes ago not sent"
            },
            _earliestEvent
        };

        _busEventSetMock = _busEvents.AsQueryable().BuildMockDbSet();

        ContextHelper
            .ContextMock
            .Setup(x => x.BusEvents)
            .Returns(_busEventSetMock.Object);

        var configuration = new Mock<IPlantService>();
        configuration.Setup(c => c.GetConfiguration()["MessageChunkSize"]).Returns("5");

        _dut = new BusEventRepository(ContextHelper.ContextMock.Object, configuration.Object);
    }
}