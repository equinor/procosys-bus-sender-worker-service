using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class QueryEventTests
{
    [TestMethod]
    public void IQueryEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var queryEventInterfaceType = typeof(IQueryEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "QueryId", typeof(long) },
            { "QueryNo", typeof(string) },
            { "Title", typeof(string) },
            { "DisciplineCode", typeof(string) },
            { "QueryType", typeof(string) },
            { "CostImpact", typeof(string) },
            { "Description", typeof(string) },
            { "Consequence", typeof(string) },
            { "ProposedSolution", typeof(string) },
            { "EngineeringReply", typeof(string) },
            { "Milestone", typeof(string) },
            { "ScheduleImpact", typeof(bool) },
            { "PossibleWarrantyClaim", typeof(bool) },
            { "IsVoided", typeof(bool) },
            { "RequiredDate", typeof(DateOnly?) },
            { "CreatedAt", typeof(DateTime) },
            { "LastUpdated", typeof(DateTime) },
        };

        // Act
        var actualProperties = queryEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}