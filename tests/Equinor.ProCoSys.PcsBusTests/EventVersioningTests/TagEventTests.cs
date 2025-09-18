using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class TagEventTests
{
    [TestMethod]
    public void ITagEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var tagEventInterfaceType = typeof(ITagEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "PlantName", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "TagId", typeof(long) },
            { "TagNo", typeof(string) },
            { "CommPkgGuid", typeof(Guid?) },
            { "CommPkgNo", typeof(string) },
            { "McPkgGuid", typeof(Guid?) },
            { "McPkgNo", typeof(string) },
            { "Description", typeof(string) },
            { "ProjectName", typeof(string) },
            { "AreaCode", typeof(string) },
            { "AreaDescription", typeof(string) },
            { "DisciplineCode", typeof(string) },
            { "DisciplineDescription", typeof(string) },
            { "RegisterCode", typeof(string) },
            { "InstallationCode", typeof(string) },
            { "Status", typeof(string) },
            { "System", typeof(string) },
            { "CallOffNo", typeof(string) },
            { "CallOffGuid", typeof(Guid?) },
            { "PurchaseOrderNo", typeof(string) },
            { "TagFunctionCode", typeof(string) },
            { "EngineeringCode", typeof(string) },
            { "ContractorCode", typeof(string) },
            { "MountedOn", typeof(int?) },
            { "MountedOnGuid", typeof(Guid?) },
            { "IsVoided", typeof(bool) },
            { "LastUpdated", typeof(DateTime) },
            { "TagDetails", typeof(string) }
        };

        // Act
        Dictionary<string, Type> actualProperties = tagEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys, EventVersioningError.ErrorMessage);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key], EventVersioningError.ErrorMessage);
    }
}