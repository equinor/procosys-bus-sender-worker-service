using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class WorkOrderMaterialEventTests
{
    [TestMethod]
    public void IWorkOrderMaterialEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var workOrderMaterialEventInterfaceType = typeof(IWorkOrderMaterialEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            {"Plant", typeof(string)},
            {"ProCoSysGuid", typeof(Guid)},
            {"ProjectName", typeof(string)},
            {"WoNo", typeof(string)},
            {"WoId", typeof(long)},
            {"WoGuid", typeof(Guid)},
            {"ItemNo", typeof(string)},
            {"TagNo", typeof(string)},
            {"TagId", typeof(long?)},
            {"TagGuid", typeof(Guid?)},
            {"TagRegisterCode", typeof(string)},
            {"StockId", typeof(long?)},
            {"Quantity", typeof(double?)},
            {"UnitName", typeof(string)},
            {"UnitDescription", typeof(string)},
            {"AdditionalInformation", typeof(string)},
            {"RequiredDate", typeof(DateOnly?)},
            {"EstimatedAvailableDate", typeof(DateOnly?)},
            {"Available", typeof(bool?)},
            {"MaterialStatus", typeof(string)},
            {"StockLocation", typeof(string)},
            {"LastUpdated", typeof(DateTime)},
        };

        // Act
        var actualProperties = workOrderMaterialEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}