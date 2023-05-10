using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class LibraryFieldEventVersioningTests
{
    [TestMethod]
    public void ILibraryFieldEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var libraryFieldEventInterfaceType = typeof(ILibraryFieldEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "LibraryGuid", typeof(Guid) },
            { "LibraryType", typeof(string) },
            { "Code", typeof(string) },
            { "ColumnName", typeof(string) },
            { "ColumnType", typeof(string) },
            { "StringValue", typeof(string) },
            { "DateValue", typeof(DateOnly?) },
            { "NumberValue", typeof(double?) },
            { "LibraryValueGuid", typeof(Guid?) },
            { "LastUpdated", typeof(DateTime) },
        };

        // Act
        var actualProperties = libraryFieldEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}