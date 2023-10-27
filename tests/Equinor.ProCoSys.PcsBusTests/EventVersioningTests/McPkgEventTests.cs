using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class McPkgEventVersioningTests
{
    [TestMethod]
    public void IMcPkgEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var mcPkgEventInterfaceType = typeof(IMcPkgEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "PlantName", typeof(string) },
            { "ProjectName", typeof(string) },
            { "McPkgNo", typeof(string) },
            { "McPkgId", typeof(long) },
            { "CommPkgNo", typeof(string) },
            { "CommPkgGuid", typeof(Guid) },
            { "Description", typeof(string) },
            { "Remark", typeof(string) },
            { "ResponsibleCode", typeof(string) },
            { "ResponsibleDescription", typeof(string) },
            { "AreaCode", typeof(string) },
            { "AreaDescription", typeof(string) },
            { "Discipline", typeof(string) },
            { "DisciplineCode", typeof(string) },
            { "McStatus", typeof(string) },
            { "Phase", typeof(string) },
            { "IsVoided", typeof(bool) },
            { "CreatedAt", typeof(DateTime) },
            { "LastUpdated", typeof(DateTime) }
        };

        // Act
        Dictionary<string, Type> actualProperties = mcPkgEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}