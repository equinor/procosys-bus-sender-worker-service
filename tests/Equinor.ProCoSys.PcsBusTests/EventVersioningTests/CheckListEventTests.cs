using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class CheckListEventTests
{
    /**
     * If this tests fails, its most likely because the versioning contract is breached. Consider creating a new version instead of
     * modifying the existing one.
     * If new properties are added to the interface (non breaking), this test should be updated with the new properties,
     * it should however not fail.
     * *
     */
    [TestMethod]
    public void ICheckListEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var checklistEventInterfaceType = typeof(IChecklistEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "TagNo", typeof(string) },
            { "TagId", typeof(long) },
            { "TagGuid", typeof(Guid) },
            { "TagRegisterId", typeof(long) },
            { "ChecklistId", typeof(long) },
            { "TagCategory", typeof(string) },
            { "SheetNo", typeof(string) },
            { "SubSheetNo", typeof(string) },
            { "FormularType", typeof(string) },
            { "FormularGroup", typeof(string) },
            { "FormPhaseCode", typeof(string) },
            { "FormPhaseGuid", typeof(Guid?) },
            { "SystemModule", typeof(string) },
            { "FormularDiscipline", typeof(string) },
            { "Revision", typeof(string) },
            { "PipingRevisionMcPkNo", typeof(string) },
            { "PipingRevisionMcPkGuid", typeof(Guid?) },
            { "ResponsibleCode", typeof(string) },
            { "ResponsibleGuid", typeof(Guid) },
            { "StatusCode", typeof(string) },
            { "StatusGuid", typeof(Guid) },
            { "UpdatedAt", typeof(DateTime?) },
            { "LastUpdated", typeof(DateTime) },
            { "CreatedAt", typeof(DateTime) },
            { "SignedAt", typeof(DateTime?) },
            { "VerifiedAt", typeof(DateTime?) }
        };
        // Act
        Dictionary<string, Type> actualProperties = checklistEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}