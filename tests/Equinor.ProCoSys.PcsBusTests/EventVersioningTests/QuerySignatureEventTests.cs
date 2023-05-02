﻿using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class QuerySignatureEventTests
{
    [TestMethod]
    public void IQuerySignatureEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var querySignatureEventInterfaceType = typeof(IQuerySignatureEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            {"Plant", typeof(string)},
            {"ProCoSysGuid", typeof(Guid)},
            {"PlantName", typeof(string)},
            {"ProjectName", typeof(string)},
            {"Status", typeof(string)},
            {"LibraryStatusGuid", typeof(Guid?)},
            {"QuerySignatureId", typeof(long)},
            {"QueryId", typeof(long)},
            {"QueryGuid", typeof(Guid)},
            {"QueryNo", typeof(string)},
            {"SignatureRoleCode", typeof(string)},
            {"FunctionalRoleCode", typeof(string)},
            {"Sequence", typeof(int)},
            {"SignedByAzureOid", typeof(Guid?)},
            {"FunctionalRoleDescription", typeof(string)},
            {"SignedDate", typeof(DateTime?)},
            {"LastUpdated", typeof(DateTime)},
        };

        // Act
        var actualProperties = querySignatureEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}