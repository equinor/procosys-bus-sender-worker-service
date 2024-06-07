using System;
using System.Collections.Generic;
using System.Linq;
using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class IpoParticipantEventTests
{
    [TestMethod]
    public void IIpoParticipantEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var ipoParticipantEventInterfaceType = typeof(IIpoParticipantEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "Organization", typeof(string) },
            { "Type", typeof(string) },
            { "FunctionalRoleCode", typeof(string) },
            { "AzureOid", typeof(Guid?) },
            { "SortKey", typeof(int) },
            { "CreatedAtUtc", typeof(DateTime) },
            { "InvitationGuid", typeof(Guid) },
            { "ModifiedAtUtc", typeof(DateTime?) },
            { "Attended", typeof(bool) },
            { "Note", typeof(string) },
            { "SignedAtUtc", typeof(DateTime?) },
            { "SignedByOid", typeof(Guid?) },
        };

        // Act
        Dictionary<string, Type> actualProperties = ipoParticipantEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}