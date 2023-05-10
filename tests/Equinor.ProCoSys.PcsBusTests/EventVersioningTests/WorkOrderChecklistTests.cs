using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class WorkOrderChecklistEventTests
{
    [TestMethod]
    public void IWorkOrderChecklistEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var workOrderChecklistEventInterfaceType = typeof(IWorkOrderChecklistEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "ChecklistId", typeof(long) },
            { "ChecklistGuid", typeof(Guid) },
            { "WoId", typeof(long) },
            { "WoGuid", typeof(Guid) },
            { "WoNo", typeof(string) },
            { "LastUpdated", typeof(DateTime) },
        };

        // Act
        var actualProperties = workOrderChecklistEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}