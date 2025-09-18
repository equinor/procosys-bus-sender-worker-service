using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class WorkOrderCutoffEventTests
{
    [TestMethod]
    public void IWorkOrderCutoffEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var workOrderCutoffEventInterfaceType = typeof(IWorkOrderCutoffEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "PlantName", typeof(string) },
            { "WoGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "WoNo", typeof(string) },
            { "JobStatusCode", typeof(string) },
            { "MaterialStatusCode", typeof(string) },
            { "DisciplineCode", typeof(string) },
            { "CategoryCode", typeof(string) },
            { "MilestoneCode", typeof(string) },
            { "SubMilestoneCode", typeof(string) },
            { "HoldByCode", typeof(string) },
            { "PlanActivityCode", typeof(string) },
            { "ResponsibleCode", typeof(string) },
            { "LastUpdated", typeof(DateTime) },
            { "CutoffWeek", typeof(int) },
            { "CutoffDate", typeof(DateOnly) },
            { "PlannedStartAtDate", typeof(DateOnly?) },
            { "PlannedFinishedAtDate", typeof(DateOnly?) },
            { "ExpendedManHours", typeof(double?) },
            { "ManHoursEarned", typeof(double?) },
            { "EstimatedHours", typeof(double?) },
            { "ManHoursExpendedLastWeek", typeof(double?) },
            { "ManHoursEarnedLastWeek", typeof(double?) },
            { "ProjectProgress", typeof(double?) }
        };

        // Act
        Dictionary<string, Type> actualProperties = workOrderCutoffEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys, EventVersioningError.ErrorMessage);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key], EventVersioningError.ErrorMessage);
    }
}