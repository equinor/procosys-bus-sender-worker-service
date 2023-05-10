using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

/**If this tests fails, its most likely because the versioning contract is breached. Consider creating a new version instead of
modifying the existing one.
If new properties are added to the interface (non breaking), this test should be updated with the new properties,
it should however not fail.
**/
[TestClass]
public class WorkOrderMilestoneEventVersioningTests
{
    [TestMethod]
    public void IWorkOrderMilestoneEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var workOrderMilestoneEventInterfaceType = typeof(IWorkOrderMilestoneEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "WoId", typeof(long) },
            { "WoGuid", typeof(Guid?) },
            { "WoNo", typeof(string) },
            { "Code", typeof(string) },
            { "MilestoneDate", typeof(DateOnly?) },
            { "SignedByAzureOid", typeof(string) },
            { "LastUpdated", typeof(DateTime) },
        };

        // Act
        var actualProperties = workOrderMilestoneEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);


        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}