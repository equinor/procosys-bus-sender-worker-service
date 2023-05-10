using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class CommPkgOperationEventTests
{
    [TestMethod]
    public void ICommPkgOperationV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var commPkgOperationEventInterfaceType = typeof(ICommPkgOperationEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "CommPkgNo", typeof(string) },
            { "CommPkgGuid", typeof(Guid) },
            { "InOperation", typeof(bool) },
            { "ReadyForProduction", typeof(bool) },
            { "MaintenanceProgram", typeof(bool) },
            { "YellowLine", typeof(bool) },
            { "BlueLine", typeof(bool) },
            { "YellowLineStatus", typeof(string) },
            { "BlueLineStatus", typeof(string) },
            { "TemporaryOperationEst", typeof(bool) },
            { "PmRoutine", typeof(bool) },
            { "CommissioningResp", typeof(bool) },
            { "ValveBlindingList", typeof(bool?) },
            { "LastUpdated", typeof(DateTime) },
        };

        // Act
        var actualProperties = commPkgOperationEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}