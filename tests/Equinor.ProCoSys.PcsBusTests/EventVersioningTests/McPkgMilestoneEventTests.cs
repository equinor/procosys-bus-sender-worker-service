using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class McPkgMilestoneEventTests
{
    [TestMethod]
    public void IMcPkgMilestoneV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var mcPkgMilestoneEventInterfaceType = typeof(IMcPkgMilestoneEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "PlantName", typeof(string) },
            { "ProjectName", typeof(string) },
            { "McPkgGuid", typeof(Guid) },
            { "McPkgNo", typeof(string) },
            { "Code", typeof(string) },
            { "ActualDate", typeof(DateTime?) },
            { "PlannedDate", typeof(DateOnly?) },
            { "ForecastDate", typeof(DateOnly?) },
            { "Remark", typeof(string) },
            { "IsSent", typeof(bool?) },
            { "IsAccepted", typeof(bool?) },
            { "IsRejected", typeof(bool?) },
            { "LastUpdated", typeof(DateTime) }
        };

        // Act
        Dictionary<string, Type> actualProperties = mcPkgMilestoneEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys, EventVersioningError.ErrorMessage);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key], EventVersioningError.ErrorMessage);
    }
}