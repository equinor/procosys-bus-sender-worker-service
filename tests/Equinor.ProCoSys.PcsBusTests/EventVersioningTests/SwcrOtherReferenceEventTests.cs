using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class SwcrOtherReferenceEventTests
{
    [TestMethod]
    public void ISwcrOtherReferenceEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var swcrOtherReferenceEventInterfaceType = typeof(ISwcrOtherReferenceEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "LibraryGuid", typeof(Guid) },
            { "SwcrGuid", typeof(Guid) },
            { "Code", typeof(string) },
            { "Description", typeof(string) },
            { "LastUpdated", typeof(DateTime) },
        };

        // Act
        var actualProperties = swcrOtherReferenceEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}