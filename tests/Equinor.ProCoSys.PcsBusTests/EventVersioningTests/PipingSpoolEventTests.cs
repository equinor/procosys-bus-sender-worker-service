using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class PipingSpoolEventTests
{
    [TestMethod]
    public void IPipingSpoolEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var pipingSpoolEventInterfaceType = typeof(IPipingSpoolEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "PipingSpoolId", typeof(int) },
            { "PipingRevisionId", typeof(int) },
            { "PipingRevisionGuid", typeof(Guid) },
            { "Revision", typeof(int) },
            { "McPkgNo", typeof(string) },
            { "McPkgGuid", typeof(Guid) },
            { "ISODrawing", typeof(string) },
            { "Spool", typeof(string) },
            { "LineNo", typeof(string) },
            { "LineGuid", typeof(Guid) },
            { "N2HeTest", typeof(bool?) },
            { "AlternativeTest", typeof(bool?) },
            { "AlternativeTestNoOfWelds", typeof(int?) },
            { "Installed", typeof(bool) },
            { "Welded", typeof(bool?) },
            { "WeldedDate", typeof(DateOnly?) },
            { "PressureTested", typeof(bool?) },
            { "NDE", typeof(bool?) },
            { "Primed", typeof(bool?) },
            { "Painted", typeof(bool?) },
            { "LastUpdated", typeof(DateTime) }
        };


        // Act
        Dictionary<string, Type> actualProperties = pipingSpoolEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys, EventVersioningError.ErrorMessage);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key], EventVersioningError.ErrorMessage);
    }
}