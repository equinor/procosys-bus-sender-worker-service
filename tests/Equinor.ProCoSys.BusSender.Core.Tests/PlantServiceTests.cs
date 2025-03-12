using System;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;


[TestClass]
public class PlantServiceTests
{
    private Mock<ILogger<PlantService>> _loggerMock;
    private Mock<IPlantRepository> _plantRepositoryMock;
    private Mock<IConfiguration> _configurationMock;
    private Mock<IMemoryCache> _cacheMock;
    private Mock<PlantService> _plantServiceMock;


    [TestInitialize]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<PlantService>>();
        _plantRepositoryMock = new Mock<IPlantRepository>();
        _configurationMock = new Mock<IConfiguration>();
        _cacheMock = new Mock<IMemoryCache>();

        _plantServiceMock = new Mock<PlantService>(_loggerMock.Object, _plantRepositoryMock.Object, _configurationMock.Object, _cacheMock.Object) { CallBase = true }; // Partial mock.


        //_configurationMock.SetupGet(x => x["MaxBlobReleaseLeaseAttempts"]).Returns("3");
        //_configurationMock.SetupGet(x => x["BlobLeaseExpiryTime"]).Returns("60");
        //_configurationMock.SetupGet(x => x["BlobReleaseLeaseDelay"]).Returns("2");

        var allPlantsInDatabase = new List<string>() { "PCS$PlantA", "PCS$PlantB", "PCS$PlantC", "PCS$PlantD" };
        _plantServiceMock.Setup(m => m.GetAllPlants()).Returns(allPlantsInDatabase);
    }

    [TestMethod]
    public void GetPlantsHandledByInstance_WhenRemainingPlant_ShouldReturnItemsForAssignedAndRemainingPlantsOnly()
    {
        var plantLeases = new List<PlantLease>()
        {
            new PlantLease()
            {
                IsCurrent = true,
                Plant = "REMAININGPLANTS"
            },
            new PlantLease()
            {
                IsCurrent = false,
                Plant = "PCS$PlantB"
            },
            new PlantLease()
            {
                IsCurrent = false,
                Plant = "PCS$PlantC"
            }
        };

        var plantsHandledByInstance = _plantServiceMock.Object.GetPlantsForCurrent(plantLeases);

        // Arrange

        Assert.IsTrue(plantsHandledByInstance[0] == "PCS$PlantA");
        Assert.IsTrue(plantsHandledByInstance[1] == "PCS$PlantD");
    }

    [TestMethod]
    public void GetPlantsHandledByInstance_WhenPlantOverlap_ShouldThrowException()
    {
        var plantLeases = new List<PlantLease>()
        {
            new PlantLease()
            {
                IsCurrent = true,
                Plant = "PCS$PlantB,PCS$PlantC"
            },
            new PlantLease()
            {
                IsCurrent = false,
                Plant = "PCS$PlantC,PCS$PlantD"
            }
        };

        // Arrange

        Assert.ThrowsException<Exception>(() =>
            _plantServiceMock.Object.GetPlantsForCurrent(plantLeases));
    }

    [TestMethod]
    public void GetPlantsHandledByInstance_WhenPlantOverlapAndPlaceholder_ShouldThrowException()
    {
        var plantLeases = new List<PlantLease>()
        {
            new PlantLease()
            {
                IsCurrent = true,
                Plant = "NOPLANT,REMAININGPLANTS,PCS$PlantB,PCS$PlantC"
            },
            new PlantLease()
            {
                IsCurrent = false,
                Plant = "PCS$PlantC,PCS$PlantD"
            }
        };

        // Arrange

        Assert.ThrowsException<Exception>(() =>
            _plantServiceMock.Object.GetPlantsForCurrent(plantLeases));
    }

    [TestMethod]
    public void GetPlantsHandledByInstance_WhenExplicitPlantAndPlaceholder_ShouldResolve()
    {
        var plantLeases = new List<PlantLease>()
        {
            new PlantLease()
            {
                IsCurrent = true,
                Plant = "NOPLANT,REMAININGPLANTS,PCS$PlantB,PCS$PlantC"
            },
            new PlantLease()
            {
                IsCurrent = false,
                Plant = "PCS$PlantX,PCS$PlantY"
            }
        };

        var plantsHandledByInstance = _plantServiceMock.Object.GetPlantsForCurrent(plantLeases);

        // Arrange

        Assert.IsTrue(plantsHandledByInstance[0] == "NOPLANT");
        Assert.IsTrue(plantsHandledByInstance[1] == "PCS$PlantB");
        Assert.IsTrue(plantsHandledByInstance[2] == "PCS$PlantC");
        Assert.IsTrue(plantsHandledByInstance[3] == "PCS$PlantA");
        Assert.IsTrue(plantsHandledByInstance[4] == "PCS$PlantD");
    }


    [TestMethod]
    public void GetPlantsHandledByInstance_WhenInvalidPlantOnly_ShouldThrowException()
    {
        var plantLeases = new List<PlantLease>()
        {
            new PlantLease()
            {
                IsCurrent = true,
                Plant = "INVALIDPLANT"
            },
            new PlantLease()
            {
                IsCurrent = false,
                Plant = "PCS$PlantB"
            },
            new PlantLease()
            {
                IsCurrent = false,
                Plant = "PCS$PlantC"
            }
        };

        Assert.ThrowsException<Exception>(() =>
            _plantServiceMock.Object.GetPlantsForCurrent(plantLeases));
    }
}