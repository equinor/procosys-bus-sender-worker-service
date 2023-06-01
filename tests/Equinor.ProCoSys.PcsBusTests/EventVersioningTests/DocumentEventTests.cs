using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class DocumentEventTests
{
    [TestMethod]
    public void IDocumentEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var documentEventInterfaceType = typeof(IDocumentEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            {"Plant", typeof(string)},
            {"ProCoSysGuid", typeof(Guid)},
            {"ProjectName", typeof(string)},
            {"DocumentId", typeof(long)},
            {"DocumentNo", typeof(string)},
            {"Title", typeof(string)},
            {"AcceptanceCode", typeof(string)},
            {"Archive", typeof(string)},
            {"AccessCode", typeof(string)},
            {"Complex", typeof(string)},
            {"DocumentType", typeof(string)},
            {"DisciplineId", typeof(string)},
            {"DocumentCategory", typeof(string)},
            {"HandoverStatus", typeof(string)},
            {"RegisterType", typeof(string)},
            {"RevisionNo", typeof(string)},
            {"RevisionStatus", typeof(string)},
            {"ResponsibleContractor", typeof(string)},
            {"LastUpdated", typeof(DateTime)},
            {"RevisionDate", typeof(DateOnly?)}
        };

        // Act
        var actualProperties = documentEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (var expectedProperty in expectedProperties)
        {
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
        }
    }
}