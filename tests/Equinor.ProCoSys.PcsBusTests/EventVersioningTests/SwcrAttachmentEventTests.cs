using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class AttachmentEventTests
{
    [TestMethod]
    public void IAttachmentEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var attachmentEventInterfaceType = typeof(IAttachmentEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "SwcrGuid", typeof(Guid?) },
            { "Title", typeof(string) },
            { "ClassificationCode", typeof(string) },
            { "Uri", typeof(string) },
            { "FileName", typeof(string) },
            { "LastUpdated", typeof(DateTime) }
        };

        // Act
        Dictionary<string, Type> actualProperties = attachmentEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}