using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class LibraryToLibraryEventTests
{
    [TestMethod]
    public void ILibraryToLibraryEventV1_ShouldNotChange()
    {
        // Arrange
        var interfaceType = typeof(ILibraryToLibraryEventV1);
        
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "Role", typeof(string) },
            { "Association", typeof(string) },
            { "LibraryGuid", typeof(Guid) },
            { "RelatedLibraryGuid", typeof(Guid) },
            { "LastUpdated", typeof(DateTime) }
        };

        // Act
        var actualProperties = interfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys, EventVersioningError.ErrorMessage);
        foreach (var expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key], EventVersioningError.ErrorMessage);
    }
    
}
