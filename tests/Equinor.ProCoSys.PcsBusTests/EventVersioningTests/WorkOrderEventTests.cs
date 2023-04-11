using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class WorkOrderEventVersioningTests
{

    /**If this tests fails, its most likely because the versioning contract is breached. Consider creating a new version instead of
    modifying the existing one.
    If new properties are added to the interface (non breaking), this test should be updated with the new properties,
    it should however not fail.
    **/
    [TestMethod]
    public void IWorkOrderEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var workOrderEventInterfaceType = typeof(IWorkOrderEventV1);

        //Nullability is not checked for string/string?
        var expectedProperties = new Dictionary<string, Type>
        {
            {"Plant", typeof(string)},
            {"ProCoSysGuid", typeof(Guid)},
            {"ProjectName", typeof(string)},
            {"WoNo", typeof(string)},
            {"WoId", typeof(long)},
            {"CommPkgNo", typeof(string)},
            {"CommPkgGuid", typeof(Guid?)},
            {"Title", typeof(string)},
            {"Description", typeof(string)},
            {"MilestoneCode", typeof(string)},
            {"SubMilestoneCode", typeof(string)},
            {"MilestoneDescription", typeof(string)},
            {"CategoryCode", typeof(string)},
            {"MaterialStatusCode", typeof(string)},
            {"HoldByCode", typeof(string)},
            {"DisciplineCode", typeof(string)},
            {"DisciplineDescription", typeof(string)},
            {"ResponsibleCode", typeof(string)},
            {"ResponsibleDescription", typeof(string)},
            {"AreaCode", typeof(string)},
            {"AreaDescription", typeof(string)},
            {"JobStatusCode", typeof(string)},
            {"MaterialComments", typeof(string)},
            {"ConstructionComments", typeof(string)},
            {"TypeOfWorkCode", typeof(string)},
            {"OnShoreOffShoreCode", typeof(string)},
            {"WoTypeCode", typeof(string)},
            {"ProjectProgress", typeof(string)},
            {"ExpendedManHours", typeof(string)},
            {"EstimatedHours", typeof(string)},
            {"RemainingHours", typeof(string)},
            {"WBS", typeof(string)},
            {"Progress", typeof(int)},
            {"PlannedStartAtDate", typeof(DateOnly?)},
            {"ActualStartAtDate", typeof(DateOnly?)},
            {"PlannedFinishedAtDate", typeof(DateOnly?)},
            {"ActualFinishedAtDate", typeof(DateOnly?)},
            {"CreatedAt", typeof(DateTime)},
            {"IsVoided", typeof(bool)},
            {"LastUpdated", typeof(DateTime)},
        };

        // Act
        var actualProperties = workOrderEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}