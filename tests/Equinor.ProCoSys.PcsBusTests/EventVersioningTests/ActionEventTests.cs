using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class ActionEventTests
{
    [TestMethod]
    public void IActionEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var actionEventInterfaceType = typeof(IActionEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ElementContentGuid", typeof(Guid) },
            { "CommPkgNo", typeof(string) },
            { "CommPkgGuid", typeof(Guid?) },
            { "SwcrNo", typeof(string) },
            { "SwcrGuid", typeof(Guid?) },
            { "DocumentNo", typeof(string) },
            { "Description", typeof(string) },
            { "DocumentGuid", typeof(Guid?) },
            { "ActionNo", typeof(string) },
            { "Title", typeof(string) },
            { "Comments", typeof(string) },
            { "Deadline", typeof(DateOnly?) },
            { "CategoryCode", typeof(string) },
            { "CategoryGuid", typeof(Guid?) },
            { "PriorityCode", typeof(string) },
            { "PriorityGuid", typeof(Guid?) },
            { "RequestedByOid", typeof(Guid?) },
            { "ActionByOid", typeof(Guid?) },
            { "ActionByRole", typeof(string) },
            { "ActionByRoleGuid", typeof(Guid?) },
            { "ResponsibleOid", typeof(Guid?) },
            { "ResponsibleRole", typeof(string) },
            { "ResponsibleRoleGuid", typeof(Guid?) },
            { "LastUpdated", typeof(DateTime) },
            { "SignedAt", typeof(DateTime?) },
            { "SignedBy", typeof(Guid?) }
        };

        // Act
        Dictionary<string, Type> actualProperties = actionEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys, EventVersioningError.ErrorMessage);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key], EventVersioningError.ErrorMessage);
    }
}