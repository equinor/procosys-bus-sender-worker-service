using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class PunchPriorityLibRelationEventTest
{
    [TestMethod]
    public void IPunchPriorityLibRelationEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var interfaceType = typeof(IPunchPriorityLibRelationEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "CommPriorityGuid", typeof(Guid) },
            { "LastUpdated", typeof(DateTime) }
        };

        // Act
        var actualProperties = interfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}