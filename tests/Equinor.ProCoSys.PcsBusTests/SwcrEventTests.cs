using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests;

[TestClass]
public class SwcrEventTests
{
    [TestMethod]
    public void ISwcrV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var swcrEventInterfaceType = typeof(ISwcrEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "SwcrNo", typeof(string) },
            { "SwcrId", typeof(long) },
            { "CommPkgGuid", typeof(Guid?) },
            { "CommPkgNo", typeof(string) },
            { "Description", typeof(string) },
            { "Modification", typeof(string) },
            { "Priority", typeof(string) },
            { "System", typeof(string) },
            { "ControlSystem", typeof(string) },
            { "Contract", typeof(string) },
            { "Supplier", typeof(string) },
            { "Node", typeof(string) },
            { "Status", typeof(string) },
            { "CreatedAt", typeof(DateTime) },
            { "IsVoided", typeof(bool) },
            { "LastUpdated", typeof(DateTime) },
            { "DueDate", typeof(DateOnly?) },
            { "EstimatedManHours", typeof(float?) }
        };


        // Act
        Dictionary<string, Type> actualProperties = swcrEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}