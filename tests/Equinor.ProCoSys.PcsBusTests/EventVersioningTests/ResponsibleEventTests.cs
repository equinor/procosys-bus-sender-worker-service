using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class ResponsibleEventTests
{
    [TestMethod]
    public void IResponsibleEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var responsibleEventInterfaceType = typeof(IResponsibleEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ResponsibleId", typeof(long) },
            { "Code", typeof(string) },
            { "ResponsibleGroup", typeof(string) },
            { "Description", typeof(string) },
            { "IsVoided", typeof(bool) },
            { "LastUpdated", typeof(DateTime) }
        };

        // Act
        Dictionary<string, Type> actualProperties = responsibleEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}