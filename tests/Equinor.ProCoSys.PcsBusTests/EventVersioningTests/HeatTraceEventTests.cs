using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class HeatTraceEventTests
{
    [TestMethod]
    public void IHeaTraceEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var heatTraceEventInterfaceType = typeof(IHeatTraceEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "HeatTraceId", typeof(long) },
            { "CableId", typeof(long) },
            { "CableGuid", typeof(Guid) },
            { "CableNo", typeof(string) },
            { "TagId", typeof(long) },
            { "TagGuid", typeof(Guid) },
            { "TagNo", typeof(string) },
            { "SpoolNo", typeof(string) },
            { "LastUpdated", typeof(DateTime) }
        };

        // Act
        Dictionary<string, Type> actualProperties = heatTraceEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys, EventVersioningError.ErrorMessage);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key], EventVersioningError.ErrorMessage);
    }
}