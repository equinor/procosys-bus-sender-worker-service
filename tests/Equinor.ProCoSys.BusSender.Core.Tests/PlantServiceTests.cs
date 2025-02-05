using System;
using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using Equinor.ProCoSys.BusSenderWorker.Core.Interfaces;
using Equinor.ProCoSys.PcsServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;

[TestClass]
public class PlantServiceTests
{

    private PlantService GetServiceDut(List<string> allPlants, List<PlantsByInstance> plantsByInstances,
        string instanceName, int messageChunkSize)
    {
        var instanceOptionsService = new InstanceOptions
        {
            InstanceName = instanceName,
            MessageChunkSize = messageChunkSize
        };

        var mockPlantRepository = new Mock<IPlantRepository>();
        mockPlantRepository.Setup(repo => repo.GetAllPlants()).Returns(allPlants);

        var mockOptionsService = new Mock<IOptions<InstanceOptions>>();
        mockOptionsService.Setup(o => o.Value).Returns(instanceOptionsService);

        var mockConfiguration = new Mock<IConfiguration>();

        var mockPlantService = new Mock<PlantService>(
                new Mock<ILogger<PlantService>>().Object,
                mockPlantRepository.Object,
                mockOptionsService.Object,
                mockConfiguration.Object
            )
            { CallBase = true };

        mockPlantService.Setup(service => service.GetPlantsByInstance()).Returns(plantsByInstances);

        return mockPlantService.Object;

    }

    [TestMethod]
    public void GetPlantsHandledByInstance_WhenTwoInstancesAssignedSamePlants_ShouldReturnItemsForAssignedPlants()
    {
        // Arrange
        var allPlants = new List<string>() { "PCS$PlantA", "PCS$PlantB" };
        var plantsByInstances = new List<PlantsByInstance>()
        {
            new PlantsByInstance()
            {
                InstanceName = "ServiceA",
                Value = "PCS$PlantA,PCS$PlantB"
            },
            new PlantsByInstance()
            {
                InstanceName = "ServiceB",
                Value = "PCS$PlantA,PCS$PlantB"
            }
        };
        var serviceADut = GetServiceDut(allPlants, plantsByInstances, "ServiceA", 200);
        var serviceBDut = GetServiceDut(allPlants, plantsByInstances, "ServiceB", 200);


        var plantsHandledByInstanceA = serviceADut.GetPlantsHandledByInstance();
        var plantsHandledByInstanceB = serviceBDut.GetPlantsHandledByInstance();

        Assert.IsTrue(plantsHandledByInstanceA[0] == "PCS$PlantA");
        Assert.IsTrue(plantsHandledByInstanceA[1] == "PCS$PlantB");
        Assert.IsTrue(plantsHandledByInstanceB[0] == "PCS$PlantA");
        Assert.IsTrue(plantsHandledByInstanceB[1] == "PCS$PlantB");
    }

    [TestMethod]
    public void GetPlantsHandledByInstance_WhenRemainingPlant_ShouldReturnItemsForAssignedAndRemainingPlantsOnly()
    {
        // Arrange
        var allPlants = new List<string>() { "PCS$PlantA", "PCS$PlantB", "PCS$PlantC", "PCS$PlantD" };
        var plantsByInstances = new List<PlantsByInstance>()
        {
            new PlantsByInstance()
            {
                InstanceName = "ServiceA",
                Value = $"PCS$PlantA,{PcsServiceBusInstanceConstants.RemainingPlants}"
            },
            new PlantsByInstance()
            {
                InstanceName = "ServiceB",
                Value = "PCS$PlantB"
            }
        };

        var serviceADut = GetServiceDut(allPlants, plantsByInstances, "ServiceA", 200);
        var plantsHandledByInstanceA = serviceADut.GetPlantsHandledByInstance();

        Assert.IsTrue(plantsHandledByInstanceA[0] == "PCS$PlantA");
        Assert.IsTrue(plantsHandledByInstanceA[1] == "PCS$PlantC");
        Assert.IsTrue(plantsHandledByInstanceA[2] == "PCS$PlantD");
    }

    [TestMethod]
    public void GetPlantsHandledByInstance_WhenInvalidPlant_ShouldReturnValidPlantsOnly()
    {
        // Arrange
        var allPlants = new List<string>() { "PCS$PlantA", "PCS$PlantB", "PCS$PlantC", "PCS$PlantD" };
        var plantsByInstances = new List<PlantsByInstance>()
        {
            new PlantsByInstance()
            {
                InstanceName = "ServiceA",
                Value = "PCS$PlantA,INVALIDPLANT,PCS$PlantB"
            }
        };

        var serviceADut = GetServiceDut(allPlants, plantsByInstances, "ServiceA", 200);
        var plantsHandledByInstanceA = serviceADut.GetPlantsHandledByInstance();

        Assert.IsTrue(plantsHandledByInstanceA[0] == "PCS$PlantA");
        Assert.IsTrue(plantsHandledByInstanceA[1] == "PCS$PlantB");
    }

    [TestMethod]
    public void GetPlantsHandledByInstance_WhenNoPlantsConfigured_ShouldReturnException()
    {
        // Arrange
        var allPlants = new List<string>() { "PCS$PlantA", "PCS$PlantB", "PCS$PlantC", "PCS$PlantD" };
        var plantsByInstances = new List<PlantsByInstance>()
        {
            new PlantsByInstance()
            {
                InstanceName = "ServiceA",
                Value = ""
            }
        };

        var serviceADut = GetServiceDut(allPlants, plantsByInstances, "ServiceA", 200);

        Assert.ThrowsException<Exception>(() =>
            serviceADut.GetPlantsHandledByInstance());
    }
}