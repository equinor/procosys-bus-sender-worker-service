using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class IpoInvitationEventTests
{
    [TestMethod]
    public void IIpoInvitationEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var ipoInvitationEventInterfaceType = typeof(IIpoInvitationEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "ProCoSysGuid", typeof(Guid) },
            { "Plant", typeof(string) },
            { "ProjectName", typeof(string) },
            { "Id", typeof(int) },
            { "CreatedAtUtc", typeof(DateTime) },
            { "CreatedByOid", typeof(Guid) },
            { "ModifiedAtUtc", typeof(DateTime?) },
            { "Title", typeof(string) },
            { "Type", typeof(string) },
            { "Description", typeof(string) },
            { "Status", typeof(string) },
            { "EndTimeUtc", typeof(DateTime) },
            { "Location", typeof(string) },
            { "StartTimeUtc", typeof(DateTime) },
            { "AcceptedAtUtc", typeof(DateTime?) },
            { "AcceptedByOid", typeof(Guid?) },
            { "CompletedAtUtc", typeof(DateTime?) },
            { "CompletedByOid", typeof(Guid?) },
        };

        // Act
        Dictionary<string, Type> actualProperties = ipoInvitationEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}