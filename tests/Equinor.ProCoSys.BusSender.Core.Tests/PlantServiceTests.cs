using Equinor.ProCoSys.BusSenderWorker.Core.Models;
using Equinor.ProCoSys.BusSenderWorker.Core.Services;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;

namespace Equinor.ProCoSys.BusSenderWorker.Core.Tests;

[TestClass]
public class PlantServiceTests
{
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

        var serviceADut = new PlantService(new Mock<ILogger<PlantService>>().Object);
        var plantsHandledByInstanceA = serviceADut.GetPlantsHandledByInstance(plantsByInstances, allPlants, "ServiceA");
        var plantsHandledByInstanceB = serviceADut.GetPlantsHandledByInstance(plantsByInstances, allPlants, "ServiceB");

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
                Value = "PCS$PlantA,REMAININGPLANTS"
            },
            new PlantsByInstance()
            {
                InstanceName = "ServiceB",
                Value = "PCS$PlantB"
            }
        };

        var serviceADut = new PlantService(new Mock<ILogger<PlantService>>().Object);
        var plantsHandledByInstance = serviceADut.GetPlantsHandledByInstance(plantsByInstances, allPlants, "ServiceA");

        Assert.IsTrue(plantsHandledByInstance[0] == "PCS$PlantA");
        Assert.IsTrue(plantsHandledByInstance[1] == "PCS$PlantC");
        Assert.IsTrue(plantsHandledByInstance[2] == "PCS$PlantD");
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

        var serviceADut = new PlantService(new Mock<ILogger<PlantService>>().Object);
        var plantsHandledByInstance = serviceADut.GetPlantsHandledByInstance(plantsByInstances, allPlants, "ServiceA");

        Assert.IsTrue(plantsHandledByInstance[0] == "PCS$PlantA");
        Assert.IsTrue(plantsHandledByInstance[1] == "PCS$PlantB");
    }
}
