using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class TagDocumentEventTests
{
    [TestMethod]
    public void ITagDocumentEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var tagEquipmentEventInterfaceType = typeof(ITagDocumentEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "TagId", typeof(long) },
            { "TagGuid", typeof(Guid) },
            { "DocumentId", typeof(long) },
            { "DocumentGuid", typeof(Guid) },
            { "LastUpdated", typeof(DateTime) }
        };

        // Act
        var actualProperties = tagEquipmentEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys, EventVersioningError.ErrorMessage);
        foreach (var expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key], EventVersioningError.ErrorMessage);
    }
}
