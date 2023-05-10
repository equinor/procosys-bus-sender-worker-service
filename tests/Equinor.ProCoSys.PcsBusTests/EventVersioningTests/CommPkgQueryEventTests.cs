using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class CommPkgQueryEventTests
{
    [TestMethod]
    public void ICommPkgQueryV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var commPkgQueryEventInterfaceType = typeof(ICommPkgQueryEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "CommPkgId", typeof(long) },
            { "CommPkgGuid", typeof(Guid) },
            { "CommPkgNo", typeof(string) },
            { "DocumentId", typeof(long) },
            { "QueryNo", typeof(string) },
            { "QueryGuid", typeof(Guid) },
            { "LastUpdated", typeof(DateTime) },
        };


        // Act
        var actualProperties = commPkgQueryEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}