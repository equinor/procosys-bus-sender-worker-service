using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class IpoCommPkgEventTests
{
    [TestMethod]
    public void IIpoCommPkgEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var ipoCommPkgEventInterfaceType = typeof(IIpoCommPkgV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "InvitationGuid", typeof(Guid) },
            { "CreatedAtUtc", typeof(DateTime) },
            { "CommPkgGuid", typeof(Guid) }
        };

        // Act
        Dictionary<string, Type> actualProperties = ipoCommPkgEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}