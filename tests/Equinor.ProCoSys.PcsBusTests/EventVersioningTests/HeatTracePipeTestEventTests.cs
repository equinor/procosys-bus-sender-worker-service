using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class HeatTracePipeTestEventTests
{
    [TestMethod]
    public void IHeatTracePipeTestEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var heatTracePipeTestEventInterfaceType = typeof(IHeatTracePipeTestEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            {"Plant", typeof(string)},
            {"ProCoSysGuid", typeof(Guid)},
            {"TagGuid", typeof(Guid)},
            {"Name", typeof(string)},
            {"LastUpdated", typeof(DateTime)}
        };

        // Act
        var actualProperties = heatTracePipeTestEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}