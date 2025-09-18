using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class CommPkgEventTests
{
    [TestMethod]
    public void ICommPkgEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var commPkgEventInterfaceType = typeof(ICommPkgEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "PlantName", typeof(string) },
            { "ProjectName", typeof(string) },
            { "ProjectGuid", typeof(Guid) },
            { "CommPkgNo", typeof(string) },
            { "CommPkgId", typeof(long) },
            { "Description", typeof(string) },
            { "CommPkgStatus", typeof(string) },
            { "IsVoided", typeof(bool) },
            { "LastUpdated", typeof(DateTime) },
            { "CreatedAt", typeof(DateTime) },
            { "DescriptionOfWork", typeof(string) },
            { "Remark", typeof(string) },
            { "ResponsibleCode", typeof(string) },
            { "ResponsibleDescription", typeof(string) },
            { "AreaCode", typeof(string) },
            { "AreaDescription", typeof(string) },
            { "Phase", typeof(string) },
            { "CommissioningIdentifier", typeof(string) },
            { "Demolition", typeof(bool?) },
            { "Priority1", typeof(string) },
            { "Priority2", typeof(string) },
            { "Priority3", typeof(string) },
            { "Progress", typeof(string) },
            { "DCCommPkgStatus", typeof(string) }
        };
        // Act
        Dictionary<string, Type> actualProperties = commPkgEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys, EventVersioningError.ErrorMessage);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key], EventVersioningError.ErrorMessage);
    }
}