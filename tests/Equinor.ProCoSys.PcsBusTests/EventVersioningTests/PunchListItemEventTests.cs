﻿using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class PunchListItemEventTests
{
    [TestMethod]
    public void IPunchListItemEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var punchListItemEventInterfaceType = typeof(IPunchListItemEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "LastUpdated", typeof(DateTime) },
            { "PunchItemNo", typeof(long) },
            { "Description", typeof(string) },
            { "ChecklistId", typeof(long) },
            { "ChecklistGuid", typeof(Guid) },
            { "Category", typeof(string) },
            { "RaisedByOrg", typeof(string) },
            { "ClearingByOrg", typeof(string) },
            { "DueDate", typeof(DateTime?) },
            { "PunchListSorting", typeof(string) },
            { "PunchListType", typeof(string) },
            { "PunchPriority", typeof(string) },
            { "Estimate", typeof(string) },
            { "OriginalWoNo", typeof(string) },
            { "OriginalWoGuid", typeof(Guid?) },
            { "WoNo", typeof(string) },
            { "WoGuid", typeof(Guid?) },
            { "SWCRNo", typeof(string) },
            { "SWCRGuid", typeof(Guid?) },
            { "DocumentNo", typeof(string) },
            { "DocumentGuid", typeof(Guid?) },
            { "ExternalItemNo", typeof(string) },
            { "MaterialRequired", typeof(bool) },
            { "IsVoided", typeof(bool) },
            { "MaterialETA", typeof(DateTime?) },
            { "MaterialExternalNo", typeof(string) },
            { "ClearedAt", typeof(DateTime?) },
            { "RejectedAt", typeof(DateTime?) },
            { "VerifiedAt", typeof(DateTime?) },
            { "CreatedAt", typeof(DateTime) },
        };

        // Act
        var actualProperties = punchListItemEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}