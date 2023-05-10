using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class CallOffEventTests
{
    [TestMethod]
    public void ICallOffEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var actionEventInterfaceType = typeof(ICallOffEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "CallOffId", typeof(long) },
            { "PackageId", typeof(long) },
            { "PurchaseOrderNo", typeof(string) },
            { "IsCompleted", typeof(bool) },
            { "UseMcScope", typeof(bool) },
            { "LastUpdated", typeof(DateTime) },
            { "IsVoided", typeof(bool) },
            { "CreatedAt", typeof(DateTime) },
            { "CallOffNo", typeof(string) },
            { "Description", typeof(string) },
            { "ResponsibleCode", typeof(string) },
            { "ContractorCode", typeof(string) },
            { "SupplierCode", typeof(string) },
            { "EstimatedTagCount", typeof(int?) },
            { "FATPlanned", typeof(DateOnly?) },
            { "PackagePlannedDelivery", typeof(DateOnly?) },
            { "PackageActualDelivery", typeof(DateOnly?) },
            { "PackageClosed", typeof(DateOnly?) },
            { "McDossierSent", typeof(DateOnly?) },
            { "McDossierReceived", typeof(DateOnly?) },
        };
        // Act
        var actualProperties = actionEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}