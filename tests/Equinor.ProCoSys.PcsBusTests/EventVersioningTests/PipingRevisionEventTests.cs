using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class PipingRevisionEventVersioningTests
{
    [TestMethod]
    public void IPipingRevisionEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var pipingRevisionEventInterfaceType = typeof(IPipingRevisionEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "PipingRevisionId", typeof(long) },
            { "Revision", typeof(int) },
            { "McPkgNo", typeof(string) },
            { "McPkgNoGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "MaxDesignPressure", typeof(double?) },
            { "MaxTestPressure", typeof(double?) },
            { "Comments", typeof(string) },
            { "TestISODocumentNo", typeof(string) },
            { "TestISODocumentGuid", typeof(Guid?) },
            { "TestISORevision", typeof(int?) },
            { "PurchaseOrderNo", typeof(string) },
            { "CallOffNo", typeof(string) },
            { "CallOffGuid", typeof(Guid?) },
            { "LastUpdated", typeof(DateTime) }
        };

        // Act
        var actualProperties = pipingRevisionEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}