using Equinor.ProCoSys.PcsServiceBus.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Equinor.ProCoSys.PcsServiceBusTests.EventVersioningTests;

[TestClass]
public class NotificationEventTest
{
    [TestMethod]
    public void INotificationEventV1_InterfacePropertiesAndMethods_DoNotChange()
    {
        // Arrange
        var queryEventInterfaceType = typeof(INotificationEventV1);
        var expectedProperties = new Dictionary<string, Type>
        {
            { "Plant", typeof(string) },
            { "ProCoSysGuid", typeof(Guid) },
            { "ProjectName", typeof(string) },
            { "NotificationId", typeof(long) },
            { "NotificationNo", typeof(string) },
            { "Title", typeof(string) },
            { "NotificationType", typeof(string) },
            { "DocumentType", typeof(string) },
            { "ResponsibleContractor", typeof(string) },
            { "CreatedAt", typeof(DateTime) },
            { "LastUpdated", typeof(DateTime) },
        };

        // Act
        Dictionary<string, Type> actualProperties = queryEventInterfaceType.GetProperties()
            .ToDictionary(p => p.Name, p => p.PropertyType);

        // Assert
        CollectionAssert.AreEquivalent(expectedProperties.Keys, actualProperties.Keys, EventVersioningError.ErrorMessage);
        foreach (KeyValuePair<string, Type> expectedProperty in expectedProperties)
            Assert.AreEqual(expectedProperty.Value, actualProperties[expectedProperty.Key], EventVersioningError.ErrorMessage);
    }
}