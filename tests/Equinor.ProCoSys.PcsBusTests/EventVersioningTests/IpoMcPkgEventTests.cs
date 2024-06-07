using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class IpoMcPkgEventTests
{
    [TestMethod]
    public void IIpoMcPkgEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var ipoMcPkgEventInterfaceType = typeof(IIpoMcPkgV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "InvitationGuid", typeof(Guid) },
            { "CreatedAtUtc", typeof(DateTime) },
            { "McPkgGuid", typeof(Guid) }
        };

        // Act
        Dictionary<string, Type> actualProperties = ipoMcPkgEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}