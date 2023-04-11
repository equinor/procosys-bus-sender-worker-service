using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class CommPkgMilestoneEventTests
{
    [TestMethod]
    public void ICommPkgMilestoneV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var commPkgMilestoneEventInterfaceType = typeof(ICommPkgMilestoneEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            {"Plant", typeof(string)},
            {"ProCoSysGuid", typeof(Guid)},
            {"PlantName", typeof(string)},
            {"ProjectName", typeof(string)},
            {"CommPkgGuid", typeof(Guid)},
            {"CommPkgNo", typeof(string)},
            {"Code", typeof(string)},
            {"ActualDate", typeof(DateTime?)},
            {"PlannedDate", typeof(DateOnly?)},
            {"ForecastDate", typeof(DateOnly?)},
            {"Remark", typeof(string)},
            {"IsSent", typeof(bool?)},
            {"IsAccepted", typeof(bool?)},
            {"IsRejected", typeof(bool?)},
            {"LastUpdated", typeof(DateTime)},
        };

        // Act
        var actualProperties = commPkgMilestoneEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}