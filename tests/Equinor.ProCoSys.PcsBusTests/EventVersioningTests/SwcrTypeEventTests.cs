﻿using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class SwcrTypeEventTests
{
    [TestMethod]
    public void ISwcrTypeEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var swcrTypeEventInterfaceType = typeof(ISwcrTypeEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "LibraryGuid", typeof(Guid) },
            { "SwcrGuid", typeof(Guid) },
            { "Code", typeof(string) },
            { "LastUpdated", typeof(DateTime) }
        };

        // Act
        Dictionary<string, Type> actualProperties = swcrTypeEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys, EventVersioningError.ErrorMessage);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key], EventVersioningError.ErrorMessage);
    }
}