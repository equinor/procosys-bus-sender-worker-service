using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class LibraryEventVersioningTests
{
    [TestMethod]
    public void ILibraryEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var libraryEventInterfaceType = typeof(ILibraryEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            {"Plant", typeof(string)},
            {"ProCoSysGuid", typeof(Guid)},
            {"LibraryId", typeof(long)},
            {"ParentId", typeof(int?)},
            {"ParentGuid", typeof(Guid?)},
            {"Code", typeof(string)},
            {"Description", typeof(string)},
            {"IsVoided", typeof(bool)},
            {"Type", typeof(string)},
            {"LastUpdated", typeof(DateTime)},
        };

        // Act
        var actualProperties = libraryEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}