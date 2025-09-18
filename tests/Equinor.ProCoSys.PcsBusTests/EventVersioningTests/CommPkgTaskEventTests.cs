﻿using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class CommPkgTaskEventTests
{
    [TestMethod]
    public void ICommPkgTaskV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var commPkgTaskEventInterfaceType = typeof(ICommPkgTaskEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProjectName", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "TaskGuid", typeof(Guid) },
            { "CommPkgGuid", typeof(Guid) },
            { "CommPkgNo", typeof(string) },
            { "LastUpdated", typeof(DateTime) }
        };

        // Act
        Dictionary<string, Type> actualProperties = commPkgTaskEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys, EventVersioningError.ErrorMessage);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key], EventVersioningError.ErrorMessage);
    }
}