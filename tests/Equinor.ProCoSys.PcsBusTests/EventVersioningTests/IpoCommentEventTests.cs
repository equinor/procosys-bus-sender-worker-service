using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class IpoCommentEventTests
{
    [TestMethod]
    public void IIpoCommentEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var ipoCommentEventInterfaceType = typeof(IIpoCommentEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "CommentText", typeof(string) },
            { "CreatedAtUtc", typeof(DateTime) },
            { "CreatedByOid", typeof(Guid) },
            { "InvitationGuid", typeof(Guid) }
        };

        // Act
        Dictionary<string, Type> actualProperties = ipoCommentEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key]);
    }
}