using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class TagEquipmentEventTests
{
    [TestMethod]
    public void ITagEquipmentEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var tagEquipmentEventInterfaceType = typeof(ITagEquipmentEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            {"Plant", typeof(string)},
            {"ProCoSysGuid", typeof(Guid)},
            {"ManufacturerName", typeof(string)},
            {"ModelNo", typeof(string)},
            {"VariantNo", typeof(string)},
            {"EqHubId", typeof(string)},
            {"SemiId", typeof(string)},
            {"ModelName", typeof(string)},
            {"ModelSubName", typeof(string)},
            {"ModelSubSubName", typeof(string)},
            {"TagGuid", typeof(Guid)},
            {"TagNo", typeof(string)},
            {"ProjectName", typeof(string)},
            {"LastUpdated", typeof(DateTime)}
        };

        // Act
        Dictionary<string, Type> actualProperties = tagEquipmentEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}